import numpy as np

# Street: 0 == Streight [0,1,0,1]
# Street: 1 == Curve    [0,0,1,1]
# Street: 2 == TSection [0,1,1,1]
# Street: 3 == 4Way     [1,1,1,1]

# 0 0 0 0 1 0 
# 0 0 2 1 3 0
# 0 0 1 0 1 0
# 0 0 1 0 1 0
# 0 0 1 0 1 0


streets = [
    {"type": 0, "orientation": 0b0101, "name": "streight" },
    {"type": 1, "orientation": 0b0011, "name": "curve"},
    {"type": 2, "orientation": 0b0111, "name": "tsection"},
    {"type": 3, "orientation": 0b1111, "name": "4way"}
]


def getValidPlace(MapArray, ValidationArray, x,y): 
    if(MapArray[x][y] == 0):
        

    return

mapArray = np.full((50,50), -1)
mapCheck = np.zero((50,50))






print(mapArray)