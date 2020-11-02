using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerateLevel
{

    public static LayoutGrid2D<int> Generate(int numberOfRooms, int roomMinWidth, int roomMaxWidth, int roomMinHeight, int roomMaxHeight)
    {
        try
        {
            var layout = LayoutGrid2D<int>.LoadFromJson(@"c:\temp\layout1.json");
            return layout;
        }
        catch
        {
            var levelGenerator = new LevelLayoutGenerator(numberOfRooms, roomMinWidth, roomMaxWidth, roomMinHeight, roomMaxHeight);
            var layout = levelGenerator.Run();
            layout.SaveToJson(@"c:\temp\layout.json");

            return layout;
        }

    }
}

public class LevelLayoutGenerator
{
    private int numberOfRooms;
    private int roomMinWidth;
    private int roomMaxWidth;
    private int roomMinHeight;
    private int roomMaxHeight;

    private const int maxAttemptsToGenerateLevel = 5;

    public LevelLayoutGenerator(int numberOfRooms, int roomMinWidth, int roomMaxWidth, int roomMinHeight, int roomMaxHeight)
    {
        this.numberOfRooms = numberOfRooms;
        this.roomMinWidth = roomMinWidth;
        this.roomMaxWidth = roomMaxWidth;
        this.roomMinHeight = roomMinHeight;
        this.roomMaxHeight = roomMaxHeight;
    }

    /// <summary>
    /// Prepare level description.
    /// </summary>
    public LevelDescriptionGrid2D<int> GetLevelDescription()
    {
        RoomDescriptionGrid2D roomDescription = GetRoomDescription();
        RoomDescriptionGrid2D startEndRoomDescription = GetStartEndRoomDescription();
        RoomDescriptionGrid2D corridorRoomDescription = GetCorridorDescription();

        var levelDescription = new LevelDescriptionGrid2D<int>();

        var i = 0;
        while (i < numberOfRooms * 2)
        {
            if (i == 0)
            {
                levelDescription.AddRoom(i, startEndRoomDescription);
            }
            else if(i == numberOfRooms * 2 - 2)
            {
                levelDescription.AddRoom(i, startEndRoomDescription);
                levelDescription.AddConnection(i - 1, i);
            }
            else
            {
                levelDescription.AddRoom(i, roomDescription);
                levelDescription.AddConnection(i - 1, i);
            }            

            i++;

            if (i != numberOfRooms * 2 - 1)
            {
                levelDescription.AddRoom(i, corridorRoomDescription);
                levelDescription.AddConnection(i - 1, i);
            }

            i++;
        }    

        return levelDescription;
    }

    private RoomDescriptionGrid2D GetCorridorDescription()
    {
        var corridorOutline = PolygonGrid2D.GetRectangle(4, 3);
        var corridorDoors = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                {
                    new DoorGrid2D(new Edgar.Geometry.Vector2Int(0, 1), new Edgar.Geometry.Vector2Int(0, 2)),
                    new DoorGrid2D(new Edgar.Geometry.Vector2Int(4, 1), new Edgar.Geometry.Vector2Int(4, 2))
                }
        );
        var corridorRoomTemplate = new RoomTemplateGrid2D(
            corridorOutline,
            corridorDoors,
            allowedTransformations: new List<TransformationGrid2D>()
            {
                    TransformationGrid2D.Identity,
                    TransformationGrid2D.Rotate90
            },
            name: "corridorRoomTemplate"
        );

        var corridorRoomTemplateLonger = new RoomTemplateGrid2D(
            PolygonGrid2D.GetRectangle(5, 2),
            new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                {
                    new DoorGrid2D(new Edgar.Geometry.Vector2Int(0, 1), new Edgar.Geometry.Vector2Int(0, 2)),
                    new DoorGrid2D(new Edgar.Geometry.Vector2Int(5, 1), new Edgar.Geometry.Vector2Int(5, 2))
                }
            ),
            allowedTransformations: new List<TransformationGrid2D>()
            {
                    TransformationGrid2D.Identity,
                    TransformationGrid2D.Rotate90
            },
            name: "corridorRoomTemplateLonger"
        );

        var corridorRoomDescription = new RoomDescriptionGrid2D(
            isCorridor: true,
            roomTemplates: new List<RoomTemplateGrid2D>() { corridorRoomTemplate, corridorRoomTemplateLonger }
);
        return corridorRoomDescription;
    }

    private RoomDescriptionGrid2D GetRoomDescription()
    {
        var roomsTemplates = new List<RoomTemplateGrid2D>();
        var doors = new SimpleDoorModeGrid2D(doorLength: 1, cornerDistance: 2);
        System.Random rnd = new System.Random();

        var transformations = new List<TransformationGrid2D>()
            {
                TransformationGrid2D.Identity,
                TransformationGrid2D.Rotate90
            };

        for (var i = 0; i <= numberOfRooms; i++)
        {
            var roomTemplate = new RoomTemplateGrid2D(
                PolygonGrid2D.GetRectangle(rnd.Next(roomMinWidth, roomMaxWidth), rnd.Next(roomMinHeight, roomMaxHeight)),
                doors,
                allowedTransformations: transformations);

            roomsTemplates.Add(roomTemplate);
        }

        var roomDescription = new RoomDescriptionGrid2D
        (
            isCorridor: false,
            roomTemplates: roomsTemplates
        );

        return roomDescription;
    }

    private RoomDescriptionGrid2D GetStartEndRoomDescription()
    {
        var roomsTemplates = new List<RoomTemplateGrid2D>();
        var doors = new SimpleDoorModeGrid2D(doorLength: 1, cornerDistance: 2);
        System.Random rnd = new System.Random();

        var transformations = new List<TransformationGrid2D>()
            {
                TransformationGrid2D.Identity,
                TransformationGrid2D.Rotate90
            };

   
            var roomTemplate = new RoomTemplateGrid2D(
                PolygonGrid2D.GetRectangle(roomMinWidth, roomMinHeight),
                doors,
                allowedTransformations: transformations);

            roomsTemplates.Add(roomTemplate);       


        var roomDescription = new RoomDescriptionGrid2D
        (
            isCorridor: false,
            roomTemplates: roomsTemplates
        );

        return roomDescription;
    }

    /// <summary>
    /// Run the generator and export the result.
    /// </summary>
    public LayoutGrid2D<int> Run()
    {
        LayoutGrid2D<int> layout = null;
        
        var levelDescription = GetLevelDescription();
        var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
        var i = 0;
        while (i < maxAttemptsToGenerateLevel)
        { 
            try
            {
                layout = generator.GenerateLayout();
                break;
            }
            catch
            {
                i++;
                if(i>= maxAttemptsToGenerateLevel)
                {
                    throw;
                }
            }
        }


        return layout;

    }
}
