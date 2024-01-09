#include <bits/stdc++.h>

using namespace std;

constexpr int MAX = 101;
constexpr double INF = 1e9+7;

// 직선
const int dx1[4] = {0, 0, 1, -1};
const int dy1[4] = {1, -1, 0, 0};

// 대각선
const int dx2[4] = {1, -1, -1, 1};
const int dy2[4] = {-1, 1, -1, 1};

using Pair = std::pair<int, int>;
using pPair = std::pair<double, Pair>;

struct Cell {
    int parentx;
    int parenty;
    double f, g, h;
};

char zmap[MAX][MAX];
int ROW = 0, COL = 0;

bool isDestination(int row, int col, Pair dst) {
	if (row == dst.first && col == dst.second) return true;
	return false;
}

bool isInRange(int row, int col) {
	return (row >= 0 && row < ROW && col >= 0 && col < COL);
}

bool isUnBlocked(std::vector<std::vector<int>>& map, int row, int col) {
	return (map[row][col] == 0);
}

double GethValue(int row, int col, Pair dst)
{
    return sqrt(pow(row - dst.first, 2) + pow(col - dst.second, 2)); // 피타고라스
}

void TraceBack(Cell cellDetails[MAX][MAX], Pair dst)
{
    stack<Pair> s;

    int y = dst.first;
    int x = dst.second;

    s.push({y, x});

    // 부모까지의 길을 되추적한다. 시작점은 parentx 랑 parenty가 각각 x, y 이므로
    // Stack 에 넣었으므로 길을 출력할때 시작점에서 시작한다.
    while(!(cellDetails[y][x].parentx == x && cellDetails[y][x].parenty == y))
    {
        int ty = cellDetails[y][x].parenty;
        int tx = cellDetails[y][x].parentx;
        y = ty;
        x = tx;
        s.push({y, x});
    }

    while(!s.empty())
    {
        zmap[s.top().first][s.top().second] = '*';
        s.pop();
    }
}

bool aStarSearch(vector<vector<int>>& map, Pair src, Pair dst)
{
    if(!isInRange(src.first, src.second) || !isInRange(dst.first, dst.second)) return false;
    if(!isUnBlocked(map, src.first, src.second) || !isUnBlocked(map, dst.first, dst.second)) return false;
    if(isDestination(src.first, src.second, dst)) return false;

    bool closedlist[MAX][MAX];
    memset(closedlist, false, sizeof(closedlist));

    Cell cellDetails[MAX][MAX];

    // 초기화, 최대 유량 알고리즘과 흡사합니다.
    // 계산해야할 값부분은 INF로하고, 계산할 경로는 -1로 초기화
    for(int i=0; i<ROW; i++)
    {
        for(int j=0; j<COL; j++)
        {
            cellDetails[i][j].f = cellDetails[i][j].g = cellDetails[i][j].h = INF;
            cellDetails[i][j].parentx = cellDetails[i][j].parenty = -1;
        }
    }

    // 시작지점
    int sy = src.first;
    int sx = src.second;
    cellDetails[sy][sx].f = cellDetails[sy][sx].g = cellDetails[sy][sx].h = 0.0;
    cellDetails[sy][sx].parentx = sx;
    cellDetails[sy][sx].parenty = sy;

    set<pPair> openList;
    openList.insert({ 0.0, { sy, sx } });

    while(!openList.empty())
    {
        pPair p = *openList.begin();
        openList.erase(openList.begin());

        int y = p.second.first;
        int x = p.second.second;
        closedlist[y][x] = true;

        double ng, nf, nh;

        // 직선
        for(int i=0; i<4; i++)
        {
            int ny = y + dy1[i];
            int nx = x + dx1[i];

            if(isInRange(ny, nx))
            {
                if(isDestination(ny, nx, dst))
                {
                    cellDetails[ny][nx].parenty = y;
                    cellDetails[ny][nx].parentx = x;
                    // 백트래킹을 실행하여 길을 찾는다.
                    TraceBack(cellDetails, dst);
                    return true;
                }
                else if(!closedlist[ny][nx] && isUnBlocked(map, ny, nx))
                {
                    ng = cellDetails[y][x].g + 1.0;
                    nh = GethValue(ny, nx, dst);
                    nf = ng + nh;

                    if(cellDetails[ny][nx].f == INF || cellDetails[ny][nx].f > nf) // 무한이거나 더 작을때니까...
                    {
                        cellDetails[ny][nx].f = nf;
                        cellDetails[ny][nx].g = ng;
                        cellDetails[ny][nx].h = nh;
                        cellDetails[ny][nx].parentx = x;
                        cellDetails[ny][nx].parenty = y;
                        openList.insert({nf, {ny, nx}});
                    }
                }
            }
        }

        // 대각선
        for(int i=0; i<4; i++)
        {
            int ny = y + dy2[i];
            int nx = x + dx2[i];

            if(isInRange(ny, nx))
            {
                if(isDestination(ny, nx, dst) && (isUnBlocked(map, y, nx) && isUnBlocked(map, ny, x)))
                {
                    cellDetails[ny][nx].parenty = y;
                    cellDetails[ny][nx].parentx = x;
                    // 백트래킹을 실행하여 길을 찾는다.
                    TraceBack(cellDetails, dst);
                    return true;
                }
                else if(!closedlist[ny][nx] && isUnBlocked(map, ny, nx))
                {
                    ng = cellDetails[y][x].g + 1.414;
                    nh = GethValue(ny, nx, dst);
                    nf = ng + nh;

                    if(cellDetails[ny][nx].f == INF || cellDetails[ny][nx].f > nf) // 무한이거나 더 작을때니까...
                    {
                        cellDetails[ny][nx].f = nf;
                        cellDetails[ny][nx].g = ng;
                        cellDetails[ny][nx].h = nh;
                        cellDetails[ny][nx].parentx = x;
                        cellDetails[ny][nx].parenty = y;
                        openList.insert({nf, {ny, nx}});
                    }
                }
            }
        }
    }
    return false;
}

void PrintMap()
{
	for(int i=0; i<ROW; i++)
    {
		for(int j=0; j<COL; j++)
		{
			cout << zmap[i][j];
		}
        cout << '\n';
	}
}

int main()
{
    Pair src, dst;
    int row, col;

	cin >> row >> col;
	ROW = row;
	COL = col;

	vector<vector<int>> grid(row, vector<int>(col));

	for(int i=0; i<row; i++)
    {
		for(int j=0; j<col; j++)
		{
			cin >> grid[i][j];

			if(grid[i][j] == 2)
            {
                src = {i, j};
                grid[i][j] = 0;
            }
            if(grid[i][j] == 3)
            {
                dst = {i, j};
                grid[i][j] = 0;
            }
		}
	}

	for(int i=0; i<row; i++)
    {
		for(int j=0; j<col; j++)
		{
			zmap[i][j] = grid[i][j] + '0';
		}
	}

	if (aStarSearch(grid, src, dst)) PrintMap();
	else cout << "실패.";
}