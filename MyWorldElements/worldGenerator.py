import numpy as np
import copy
import random
import time

# Street: 0 == Streight [0,1,0,1]
# Street: 1 == Curve    [0,0,1,1]
# Street: 2 == TSection [0,1,1,1]
# Street: 3 == 4Way     [1,1,1,1]

# 0 0 0 0 1 0 
# 0 0 2 1 3 0
# 0 0 1 0 1 0
# 0 0 1 0 1 0
# 0 0 1 0 1 0

list_of_possible_sections = [
    "s0", #streight piece I
    "s1", #streight piece -
    
    "c0", #down to right
    "c1", #right to up 
    "c2", #up to left
    "c3", #left to down
    
    "t0", #T-section pointing down
    "t1", #T-section pointing right
    "t2", #T-section pointing up
    "t3", #T-section pointing left
    
    "4w" #4way street
]

x = 800
y = 800

def createMap(x,y):
    map_array = [[0 for _ in range(x)] for _ in range(y)]
    return map_array

def display_map(map_array):
    for row in map_array:
        print(' '.join(str(cell).rjust(2) if isinstance(cell, str) and cell.startswith(('s', 'c')) else str(cell).rjust(2) for cell in row))



        
        
def placeStraight(map_array, placeX, placeY, rotation):
    #new_map = copy.deepcopy(map_array)
    new_map = map_array
    # check if valid rotation
    if rotation not in [0, 1]:
        print("ERROR!: At X " + str(placeX) + " Y " + str(placeY) + " placeStraight tried to place a piece with invalid rotation!")
        return False

    # check if location is free
    if new_map[placeY][placeX] in list_of_possible_sections:
        print("ERROR!: At X " + str(placeX) + " Y " + str(placeY) + " placeStraight tried to place a piece at an already filled position!")
        return False

    # check for new valid connection points
    if rotation == 0:  # up or down
        if 0 <= placeY + 1 < len(new_map):
            if(new_map[placeY+1][placeX] == 0): new_map[placeY+1][placeX] = 1
        if 0 <= placeY - 1 < len(new_map):
            if(new_map[placeY-1][placeX] == 0): new_map[placeY-1][placeX] = 1       
        new_map[placeY][placeX] = "s0"
        return new_map
    elif rotation == 1:  # left or right
        if 0 <= placeX + 1 < len(new_map[0]):
            if(new_map[placeY][placeX+1] == 0): new_map[placeY][placeX+1] = 1
        if 0 <= placeX - 1 < len(new_map[0]):
            if(new_map[placeY][placeX-1] == 0): new_map[placeY][placeX-1] = 1    
        new_map[placeY][placeX] = "s1"
        return new_map
    else:
        return False

def placeCurve(map_array, placeX, placeY, rotation):
    #new_map = copy.deepcopy(map_array)
    new_map = map_array
    if rotation not in [0, 1, 2, 3]:
        print("ERROR!: At X " + str(placeX) + " Y " + str(placeY) + " placeCurve tried to place a piece with invalid rotation!")
        return False

    if new_map[placeY][placeX] in list_of_possible_sections:
        print("ERROR!: At X " + str(placeX) + " Y " + str(placeY) + " placeCurve tried to place a piece at an already filled position!")
        return False

    if rotation == 0: #down to right
        if 0 <= placeY - 1 < len(new_map):
            if(new_map[placeY-1][placeX] == 0): new_map[placeY-1][placeX] = 1    
        if 0 <= placeX + 1 < len(new_map[0]):
            if(new_map[placeY][placeX+1] == 0): new_map[placeY][placeX+1] = 1        
        new_map[placeY][placeX] = "c0"
        return new_map
    elif rotation == 1: #right to up 
        if 0 <= placeX + 1 < len(new_map[0]):
            if(new_map[placeY][placeX+1] == 0): new_map[placeY][placeX+1] = 1
        if 0 <= placeY + 1 < len(new_map):
            if(new_map[placeY+1][placeX] == 0): new_map[placeY+1][placeX] = 1        
        new_map[placeY][placeX] = "c1"
        return new_map
    elif rotation == 2: #up to left
        if 0 <= placeY + 1 < len(new_map):
            if(new_map[placeY+1][placeX] == 0): new_map[placeY+1][placeX] = 1           
        if 0 <= placeX - 1 < len(new_map[0]):
            if(new_map[placeY][placeX-1] == 0): new_map[placeY][placeX-1] = 1    
        new_map[placeY][placeX] = "c2"
        return new_map
    elif rotation == 3: #left to down
        if 0 <= placeY - 1 < len(new_map):
            if(new_map[placeY-1][placeX] == 0): new_map[placeY-1][placeX] = 1   
        if 0 <= placeX - 1 < len(new_map[0]):
            if(new_map[placeY][placeX-1] == 0): new_map[placeY][placeX-1] = 1    
        new_map[placeY][placeX] = "c3"
        return new_map
    else:
        return False

def placeTSection(map_array, placeX, placeY, rotation):
    #new_map = copy.deepcopy(map_array)
    new_map = map_array
    # check if valid rotation
    if rotation not in [0, 1, 2, 3]:
        print("ERROR!: At X " + str(placeX) + " Y " + str(placeY) + " placeTSection tried to place a piece with invalid rotation!")
        return False

    # check if location is free
    if new_map[placeY][placeX] in list_of_possible_sections:
        print("ERROR!: At X " + str(placeX) + " Y " + str(placeY) + " placeTSection tried to place a piece at an already filled position!")
        return False

    # check for new valid connection points
    if rotation == 0:  # T-section pointing down
        if 0 <= placeY - 1 < len(new_map):
            if new_map[placeY - 1][placeX] == 0:
                new_map[placeY - 1][placeX] = 1
        if 0 <= placeX + 1 < len(new_map[0]):
            if new_map[placeY][placeX + 1] == 0:
                new_map[placeY][placeX + 1] = 1
        if 0 <= placeX - 1 < len(new_map[0]):
            if new_map[placeY][placeX - 1] == 0:
                new_map[placeY][placeX - 1] = 1
        new_map[placeY][placeX] = "t0"
        return new_map
    elif rotation == 1:  # T-section pointing right
        if 0 <= placeX - 1 < len(new_map[0]):
            if new_map[placeY][placeX - 1] == 0:
                new_map[placeY][placeX - 1] = 1
        if 0 <= placeY - 1 < len(new_map):
            if new_map[placeY - 1][placeX] == 0:
                new_map[placeY - 1][placeX] = 1
        if 0 <= placeY + 1 < len(new_map):
            if new_map[placeY + 1][placeX] == 0:
                new_map[placeY + 1][placeX] = 1
        new_map[placeY][placeX] = "t1"
        return new_map
    elif rotation == 2:  # T-section pointing up
        if 0 <= placeY + 1 < len(new_map):
            if new_map[placeY + 1][placeX] == 0:
                new_map[placeY + 1][placeX] = 1
        if 0 <= placeX + 1 < len(new_map[0]):
            if new_map[placeY][placeX + 1] == 0:
                new_map[placeY][placeX + 1] = 1
        if 0 <= placeX - 1 < len(new_map[0]):
            if new_map[placeY][placeX - 1] == 0:
                new_map[placeY][placeX - 1] = 1
        new_map[placeY][placeX] = "t2"
        return new_map
    elif rotation == 3:  # T-section pointing left
        if 0 <= placeX + 1 < len(new_map[0]):
            if new_map[placeY][placeX + 1] == 0:
                new_map[placeY][placeX + 1] = 1
        if 0 <= placeY - 1 < len(new_map):
            if new_map[placeY - 1][placeX] == 0:
                new_map[placeY - 1][placeX] = 1
        if 0 <= placeY + 1 < len(new_map):
            if new_map[placeY + 1][placeX] == 0:
                new_map[placeY + 1][placeX] = 1
        new_map[placeY][placeX] = "t3"
        return new_map
    else:
        return False

def place4Way(map_array, placeX, placeY):
    #new_map = copy.deepcopy(map_array)
    new_map = map_array
    # check if location is free
    if new_map[placeY][placeX] in list_of_possible_sections:
        print("ERROR!: At X " + str(placeX) + " Y " + str(placeY) + " place4Way tried to place a piece at an already filled position!")
        return False

    # check for new valid connection points
    if 0 <= placeY - 1 < len(new_map):
        if new_map[placeY - 1][placeX] == 0:
            new_map[placeY - 1][placeX] = 1
    if 0 <= placeY + 1 < len(new_map):
        if new_map[placeY + 1][placeX] == 0:
            new_map[placeY + 1][placeX] = 1
    if 0 <= placeX - 1 < len(new_map[0]):
        if new_map[placeY][placeX - 1] == 0:
            new_map[placeY][placeX - 1] = 1
    if 0 <= placeX + 1 < len(new_map[0]):
        if new_map[placeY][placeX + 1] == 0:
            new_map[placeY][placeX + 1] = 1

    new_map[placeY][placeX] = "4w"
    return new_map
    
        
#def fillMap(map_array):
    x = len(map_array[0])
    y = len(map_array)

    def is_connected(map_array):
        visited = [[False] * x for _ in range(y)]

        def dfs(y, x):
            if y < 0 or y >= len(map_array) or x < 0 or x >= len(map_array[0]) or visited[y][x] or map_array[y][x] == 0:
                return
            visited[y][x] = True
            dfs(y - 1, x)
            dfs(y + 1, x)
            dfs(y, x - 1)
            dfs(y, x + 1)

        # Find the first street and start DFS
        for i in range(y):
            for j in range(x):
                if isinstance(map_array[i][j], str) and map_array[i][j].startswith(('s', 'c', 't', '4w')):
                    dfs(i, j)
                    break

        # Check if all streets are visited
        for i in range(y):
            for j in range(x):
                if isinstance(map_array[i][j], str) and map_array[i][j].startswith(('s', 'c', 't', '4w')) and not visited[i][j]:
                    return False
        return True

    for row in range(y):
        for col in range(x):
            # Check if the current position is empty (0)
            if map_array[row][col] == 0:
                while True:
                    # Randomly choose a street type (straight, curve, T-section, 4-way)
                    street_type = random.choice(["s", "c", "t", "4w"])
                    
                    # Place the selected street segment on the map
                    if street_type == "s":
                        rotation = random.randint(0, 1)
                        new_map = placeStraight(map_array, col, row, rotation)
                    elif street_type == "c":
                        rotation = random.randint(0, 3)
                        new_map = placeCurve(map_array, col, row, rotation)
                    elif street_type == "t":
                        rotation = random.randint(0, 3)
                        new_map = placeTSection(map_array, col, row, rotation)
                    elif street_type == "4w":
                        new_map = place4Way(map_array, col, row)

                    # Check if the placement ensures connectivity
                    if is_connected(new_map):
                        map_array = new_map
                        break

    return map_array
        
def fillMap(map_array):
    x = len(map_array[0])
    y = len(map_array)

    for row in range(y):
        for col in range(x):
            # Check if the current position is empty (0)
            if map_array[row][col] == 0:
                # Randomly choose a street type (straight, curve, T-section, 4-way)
                street_type = random.choice(["s", "c", "t", "4w"])
                
                # Place the selected street segment on the map
                if street_type == "s":
                    rotation = random.randint(0, 1)
                    map_array = placeStraight(map_array, col, row, rotation)
                elif street_type == "c":
                    rotation = random.randint(0, 3)
                    map_array = placeCurve(map_array, col, row, rotation)
                elif street_type == "t":
                    rotation = random.randint(0, 3)
                    map_array = placeTSection(map_array, col, row, rotation)
                elif street_type == "4w":
                    map_array = place4Way(map_array, col, row)

    return map_array
        
def main(): 
    mapArray = createMap(x,y)
    
    #newMap = placeStraight(mapArray, 5, 5, 0)
    #newMap = placeCurve(newMap, 5, 6, 3) #4,5,2
    #newMap = placeTSection(newMap,4,6,1)
    #newMap = place4Way(newMap, 3,6)
    start_time = time.time()
    filledMap = fillMap(mapArray)
    end_time = time.time()
    
    
    display_map(filledMap)
    print(f"Elapsed time: {end_time-start_time}")





if __name__ == "__main__":
    main()