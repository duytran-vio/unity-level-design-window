# Unity Level Design Window.

## How to open.
    Toolbar -> Extend -> LevelDesignWindow.

## Level list section

### New level button.
    Create a brand new level.
### Level list.
    - Consist of all level data files which are created by this window.
    - Those files are located in directory: Assets/Resources/LevelDesign.
## Level Customize tab
    - Size X and Y: are the size of level.
    - Obstacle Percent: the proportion of obstacle cell in level. This value is used for generate random level.
    - `Random Level` button: Generate a random level (guarantee that this level have a path between start and end location).
    - Start and End position: choose start and end coordination (can also generate random coordinations via ```Random Start or End``` button).
    - Change default cell: change default cell which is used for extend the level.
    - Cell options: are used for editing the level.
    - Save file: Enter save name in **Save file name** box, click ```Save Level```.
    - Reset level: choose to reset all above value.
## Solve level section.
    - **Min path cost**: the minimum amount of cost of the path from start to end.
    - **Min path steps**: the minimum number of steps from start to end.
    - **Attempted steps**: the number of steps that AI use to find the minimum path.
    - **Level Difficulty**: the value to evaluate the difficulty of level.
    - ```Show attempted Steps```: show attempted steps on preview level section.
    - ```Show min Path```: show the minimum path on preview level section.
    - ```Original map```: show the original level on preview level section.
    - ```Export Json```: export above value and level data to a json file which is saved in directory: **Assests/Resources/LevelEval**

## View on scene button.
    Generate the level on game scene.

## Generate multi level.
    Input the number of easy or hard levels and generate random levels.
## Preview level section.
    Viewing and editing (using Level Customize tool) the level on this section.
    