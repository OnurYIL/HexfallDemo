
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HexFallDemo
{
    public class GridManager : MonoBehaviour
    {
        private CustomInputManager m_inputManager;

        public Text debugText, debugText1;
        public int totalScore;
        private bool spawnBomb = false;

        [Tooltip("The tile prefab.")]
        [SerializeField] private GameObject tilePrefab;
        [Tooltip("The bomb prefab.")]
        [SerializeField] private GameObject bombPrefab;

        [Tooltip("How many rows do the grid have? (Default:8)")]
        [SerializeField] private int rows = 8;

        [Tooltip("How many columns do the grid have? (Default:9)")]
        [SerializeField] private int columns = 9;

        [Tooltip("How many colors do the grid have?")]
        [SerializeField] private Color[] colors;

        [Tooltip("Object to be used as the center of the selection group. Needs to parent 3 border objects aswell.")]
        [SerializeField] private Transform tileGroupCenterObj;

        [SerializeField] private ParticleSystem explosion;
        private UIController m_uiController;

        public Vector2 tileSize;
        private Vector2 bottomLeft;
        private Vector2 lastTouchPosition;

        private GameObject[] selectionBorders;

        private TileMatrix gridTiles;
        private Tile[] defaultTiles;

        private List<GameObject> bombs;

        private TileGroup tileGroup;

        private Coroutine RotateCoroutine;
        private GameState _gameState;

        public delegate void OnGameStarted();
        public event OnGameStarted onGameStartedEvent;

        public delegate void OnGameFinished();
        public event OnGameFinished onGameFinishedEvent;

        // Start is called before the first frame update

        private void Awake()
        {
            _gameState = GameState.init;
            m_inputManager = GetComponent<CustomInputManager>();
            m_uiController = GameObject.FindGameObjectWithTag(Tags.UIController).GetComponent<UIController>();
            selectionBorders = new GameObject[]
            {
            tileGroupCenterObj.GetChild(0).gameObject,
            tileGroupCenterObj.GetChild(1).gameObject,
            tileGroupCenterObj.GetChild(2).gameObject
            };
        }

        void Start()
        {
            InitEventTriggers();
            InitDefaultTiles();
            CalculateGridAndTileSize();
            GenerateGrid();
        }
        /// <summary>
        /// Initializes touch events
        /// </summary>
        void InitEventTriggers()
        {
            m_inputManager.onScreenTouchEvent += OnScreenTouch;
            m_inputManager.OnScreenDragEvent += OnScreenDrag;
        }

        /// <summary>
        /// Selects the tile group on touch
        /// </summary>
        private void OnScreenTouch(Vector2 touchPos)
        {
            if (_gameState == GameState.idle)
            {
                lastTouchPosition = touchPos;
                SelectTileGroup(touchPos);
            }

        }

        /// <summary>
        /// Starts the rotation
        /// </summary>
        private void OnScreenDrag(SwipeDirection _swipeDirection)
        {
            if (_gameState == GameState.idle)
                RotateCoroutine = StartCoroutine(RotateTileGroup(_swipeDirection));
        }

        /// <summary>
        /// Rotates a tile group 
        /// </summary>
        IEnumerator RotateTileGroup(SwipeDirection _swipeDirection)
        {
            _gameState = GameState.rotating;

            SwapDirection _swapDirection = CalculateRotateDirection(_swipeDirection);
            print("Rotating...");

            //Rotating 120 degrees, 3 times if no matches found
            for (int i = 0; i < 3; i++)
            {
                tileGroup.SetParent(tileGroupCenterObj);

                float elapsedTime = 0;

                Quaternion currentRotation = tileGroupCenterObj.transform.rotation;
                Quaternion targetRotation = currentRotation;

                //Clockwise or anti-clockwise rotation
                targetRotation *= Quaternion.Euler(Vector3.forward * 120 * (int)_swapDirection);

                //Slerp rotation to have a smooth movement effect
                while (elapsedTime < Values.GroupRotationTime)
                {
                    elapsedTime += Time.deltaTime;
                    tileGroupCenterObj.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, elapsedTime / Values.GroupRotationTime);
                    yield return null;
                }

                tileGroupCenterObj.transform.rotation = targetRotation;

                //Actual swapping of the tiles on the grid matrix
                tileGroup.SwapTiles(gridTiles, _swapDirection);

                tileGroup.RemoveParent();


                TileMatches groupMatches = gridTiles.CheckTileForMaches(tileGroup);

                print("Checking group match...");

                StartCoroutine(DisableMatchedTiles(groupMatches));

                yield return new WaitForSeconds(Values.RotationDelay);

            }

            _gameState = GameState.idle;
        }


        /// <summary>
        /// Collapses the columns with empty tiles
        /// </summary>
        IEnumerator MoveDownTiles(TileMatches match)
        {
            _gameState = GameState.matching;

            foreach (var tile in match.MatchedTiles)
            {
                List<GameObject> tilesToMove = new List<GameObject>();

                for (int i = tile.GetComponent<Tile>().row; i < rows; i++)
                {
                    tilesToMove.Add(gridTiles[i, tile.GetComponent<Tile>().column]);
                }
                for (int i = 0; i < tilesToMove.Count; i++)
                {
                    if (match.MatchedTiles.Contains(tilesToMove[i]))
                        continue;

                    StartCoroutine(tile.GetComponent<Tile>().SwapDownRow(gridTiles, tilesToMove[i].GetComponent<Tile>()));

                    yield return new WaitForSeconds(Values.TileSwapDelay);
                }

                //For each removed tile, create a new one.
                InstantiateNewTile(tile.GetComponent<Tile>().row, tile.GetComponent<Tile>().column, tile.transform.position);

                Destroy(tile);
            }

            _gameState = GameState.matchingComplete;
        }

        /// <summary>
        /// Here starts the disabling of the matched tiles
        /// </summary>
        IEnumerator DisableMatchedTiles(TileMatches matches)
        {
            print("Hiding the matched ones...");
            if (matches.MatchedTiles.Count() < 3)
                yield break;

            HideSelection(true);

            print("Stopping rotation");
            StopCoroutine(RotateCoroutine);

            _gameState = GameState.matching;

            AddScore(matches.MatchedTiles.Count());

            foreach (var t in matches.MatchedTiles)
            {
                t.SetActive(false);

                /* t.GetComponent<SpriteRenderer>().color = new Color(
                     t.GetComponent<SpriteRenderer>().color.r,
                     t.GetComponent<SpriteRenderer>().color.g,
                     t.GetComponent<SpriteRenderer>().color.b, 0f
                     );*/
            }
            yield return new WaitForSeconds(Values.MatchDelay);

            StartCoroutine(MoveDownTiles(matches));

        }

        /// <summary>
        /// Calculates the rotation direction(clockwise,anticlockwise) 
        /// based on the user swipe direction and the first touch position
        /// </summary>
        SwapDirection CalculateRotateDirection(SwipeDirection _swipeDirection)
        {
            Vector2 swipeStartPosition = m_inputManager.GetSwipeStartPosition();

            print(_swipeDirection);
            switch (_swipeDirection)
            {
                case SwipeDirection.left:
                    {
                        if (swipeStartPosition.y > tileGroupCenterObj.position.y)
                            return SwapDirection.clockwise;

                        else
                            return SwapDirection.antiClockwise;
                    }
                case SwipeDirection.right:
                    {
                        if (swipeStartPosition.y > tileGroupCenterObj.position.y)
                            return SwapDirection.antiClockwise;

                        else
                            return SwapDirection.clockwise;
                    }

                case SwipeDirection.up:
                    {
                        if (swipeStartPosition.x < tileGroupCenterObj.position.x)
                            return SwapDirection.antiClockwise;

                        else
                            return SwapDirection.clockwise;
                    }
                case SwipeDirection.down:
                    {
                        if (swipeStartPosition.x < tileGroupCenterObj.position.x)
                            return SwapDirection.clockwise;

                        else
                            return SwapDirection.antiClockwise;
                    }
                default: return SwapDirection.clockwise;
            }
        }

        /// <summary>
        /// Select 3 tiles based on the touch position
        /// </summary>
        void SelectTileGroup(Vector3 centerPos)
        {
            _gameState = GameState.selecting;
            debugText.text = "";
            debugText1.text = "";

            //Casts a circle with the touch position as origin.
            List<Collider2D> candidateTiles = Physics2D.OverlapCircleAll(centerPos, tileSize.x).ToList();

            //Gets the closest 3 tiles from where the circle was cast.
            var closestTiles = candidateTiles.OrderBy(t => (t.transform.position - centerPos).sqrMagnitude)
                               .Take(3)
                               .ToArray();

            //Check the edges and make sure the 3 selected tiles are not in line(3 same rows, 3 same columns).
            var column = closestTiles.First().GetComponent<Tile>().column;
            var row = closestTiles.First().GetComponent<Tile>().row;

            //If same column
            if (closestTiles.All(x => x.GetComponent<Tile>().column == column))
            {
                candidateTiles.Remove(closestTiles[2]);
                closestTiles = candidateTiles.OrderBy(t => (t.transform.position - centerPos).sqrMagnitude)
                               .Take(3)
                               .ToArray();
            }
            //if same row
            if (closestTiles.All(x => x.GetComponent<Tile>().row == row))
            {
                candidateTiles.Remove(closestTiles[2]);
                closestTiles = candidateTiles.OrderBy(t => (t.transform.position - centerPos).sqrMagnitude)
                               .Take(3)
                               .ToArray();
            }

            List<Tile> selectedTiles = new List<Tile>(3);

            tileGroup = new TileGroup();


            tileGroupCenterObj.position = Helpers.FindGroupCenter(closestTiles);


            for (int i = 0; i < 3; i++)
            {
                selectedTiles.Add(closestTiles[i].GetComponent<Tile>());
                selectionBorders[i].transform.position = closestTiles[i].transform.position;
            }

            tileGroup.tiles = selectedTiles;

            //Reorder selected tiles to rotate them easier later.
            tileGroup.OrderTiles();
            _gameState = GameState.idle;
        }

        /// <summary>
        /// Checks the entire grid matrix for maches. 
        /// This runs after an explosion happened on the grid and new tiles are created.
        /// </summary>
        bool CheckAllTilesForMatch()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    TileMatches match = gridTiles.CheckTileForMaches(gridTiles[i, j]);
                    if (match != null && match.MatchedTiles.Count() > 2)
                    {
                        StartCoroutine(DisableMatchedTiles(match));
                        return true;
                    }
                }

            }
            //All matchings complete, ready for input


            _gameState = GameState.idle;

            OnMatchHappened();

            if (_gameState != GameState.ending)
            {
                HideSelection(false);
                SelectTileGroup(lastTouchPosition);

                if (!gridTiles.CheckPotentialMatches())
                    GameOver();
                
            }
            return false;
        }

        void AddScore(int matchedTileCount)
        {
            var score = matchedTileCount * Values.BaseScore;
            for (int i = 0; i < score; i++)
            {
                totalScore++;

                if (totalScore % 100 == 0)
                    spawnBomb = true;
            }

            m_uiController.DisplayScore(totalScore);
        }

        void OnMatchHappened()
        {
            bombs = bombs.Where(bomb => bomb != null).ToList();

            if (bombs.Count != 0)
            {
                foreach (var bomb in bombs)
                {
                    print("ticking");
                    if (bomb.GetComponent<Bomb>().Tick() == 0)
                    {
                        GameOver();
                        break;
                    }
                }
            }
        }



        /// <summary>
        /// Using dynamic grid here. Can have different row/column or screen size.
        /// This function makes sure everything fits in the screen.
        /// </summary>
        void CalculateGridAndTileSize()
        {
            float width = tilePrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            float height = tilePrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.y;


            float worldScreenHeight = Camera.main.orthographicSize * 2f;
            float worldScreenWidth = (worldScreenHeight / Screen.height) * Screen.width;

            Values.WorldScreenSize = new Vector2(worldScreenWidth, worldScreenHeight);
            float aspect = height / width;
            float newTileWidth;

            if (aspect > rows / (float)columns)
                newTileWidth = (worldScreenWidth / (columns)) * 1.1f;
            else
                newTileWidth = (worldScreenWidth / (rows)) * 1.1f;

            float newTileHeight = newTileWidth;

            float verticalDistance = Helpers.CalculateVerticalDistance(
                Helpers.CalculateHexaPosition(0, 0, newTileWidth, new Vector2(0, 0)),
                Helpers.CalculateHexaPosition(0, 1, newTileWidth, new Vector2(0, 0)));

            tileSize = new Vector2(newTileWidth, newTileHeight * height);

            //Starting position for hexagonal grid
            bottomLeft = new Vector2((-(verticalDistance * columns) / 2) + (verticalDistance / 2), (-(verticalDistance * rows) / 2) + (verticalDistance / 2) - worldScreenHeight / 8);

            Vector2 tileScale = new Vector2(newTileWidth, newTileHeight);
            tilePrefab.transform.localScale = tileScale;

            tileGroupCenterObj.transform.localScale = tileScale;

            Values.TileSize = tileSize;
        }

        /// <summary>
        /// Generates the hexagonal grid with the values calculated.
        /// </summary>
        void GenerateGrid()
        {
            gridTiles = new TileMatrix(rows, columns);

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    InstantiateNewTile(row, column, Helpers.CalculateHexaPosition(row, column, tileSize.x, bottomLeft));
                }
            }
            _gameState = GameState.idle;
        }
        /// <summary>
        /// Instantiates a new random tile
        /// </summary>
        void InstantiateNewTile(int row, int column, Vector2 pos)
        {
            print("Creating new tile...");
            Tile randomTile = GetRandomColoredTile();

            while (column >= 1 && gridTiles[row, column - 1].GetComponent<Tile>().Compare(randomTile))
            {
                randomTile = GetRandomColoredTile();
            }
            while (row >= 1 && gridTiles[row - 1, column].GetComponent<Tile>().Compare(randomTile))
            {
                randomTile = GetRandomColoredTile();
            }

            GameObject gTile;
            if (spawnBomb)
            {
                spawnBomb = false;
                gTile = Instantiate(bombPrefab,
                      pos, Quaternion.identity);

                if (bombs == null)
                    bombs = new List<GameObject>();

                bombs.Add(gTile);
            }
            else
            {
                gTile = Instantiate(tilePrefab,
                      pos, Quaternion.identity);
            }

            randomTile.row = row;
            randomTile.column = column;

            ApplyNewTile(randomTile, gTile);

            gridTiles[row, column] = gTile;
        }

        /// <summary>
        /// Applies the tile properties to the tile object(row,column,color)
        /// </summary>
        void ApplyNewTile(Tile tile, GameObject gTile)
        {
            gTile.GetComponent<Tile>().Apply(tile);
        }

        /// <summary>
        /// Since we want a totally dynamic grid, 
        /// we create 1 tile per color to choose from later on.
        /// </summary>
        void InitDefaultTiles()
        {
            bombs = new List<GameObject>();
            defaultTiles = new Tile[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                Tile tile = new Tile();
                tile.color = colors[i];
                defaultTiles[i] = tile;
            }
        }

        /// <summary>
        /// Returns a random tile from the list of previously created default tiles
        /// </summary>
        Tile GetRandomColoredTile()
        {
            return defaultTiles[Random.Range(0, defaultTiles.Length)];
        }

        /// <summary>
        /// Hides/shows the white selection border.
        /// </summary>
        void HideSelection(bool isHidden)
        {
            tileGroupCenterObj.gameObject.SetActive(!isHidden);
        }

        void GameOver()
        {
            print("gameover");
            HideSelection(true);

            _gameState = GameState.ending;
            StartCoroutine(gridTiles.CollapseAllTiles());

            onGameFinishedEvent?.Invoke();
        }

        /// <summary>
        /// Update is only used for checking all tiles when an explosion happened on the grid
        /// </summary>
        void Update()
        {
            if (_gameState == GameState.matchingComplete)
            {
                print("checkingComplete");
                _gameState = GameState.idle;
                CheckAllTilesForMatch();
            }
        }
    }
}
