using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerateLevel
{
    public static LayoutGrid2D<int> Generate(int numberOfRooms, int roomWidth, int roomHeight)
    {
        try
        {
            var layout = LayoutGrid2D<int>.LoadFromJson(@"c:\temp\layout1.json");
            return layout;
        }
        catch
        {
            var levelGenerator = new LevelLayoutGenerator(numberOfRooms, roomWidth, roomHeight);
            var layout = levelGenerator.Run();
            layout.SaveToJson(@"c:\temp\layout.json");

            return layout;
        }

    }
}

public class LevelLayoutGenerator
{
    private int numberOfRooms;
    private int roomWidth;
    private int roomHeight;

    public LevelLayoutGenerator(int numberOfRooms, int roomWidth, int roomHeight)
    {
        this.numberOfRooms = numberOfRooms;
        this.roomWidth = roomWidth;
        this.roomHeight = roomHeight;
    }

    /// <summary>
    /// Prepare level description.
    /// </summary>
    public LevelDescriptionGrid2D<int> GetLevelDescription()
    {
        RoomDescriptionGrid2D roomDescription = GetRoomDescription();
        RoomDescriptionGrid2D corridorRoomDescription = GetCorridorDescription();

        var levelDescription = new LevelDescriptionGrid2D<int>();

        var i = 0;
        while (i < numberOfRooms * 2)
        {

            levelDescription.AddRoom(i, roomDescription);
            if (i != 0)
            {
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
        //levelDescription.AddRoom(0, roomDescription);
        //levelDescription.AddRoom(1, corridorRoomDescription);
        //levelDescription.AddRoom(2, roomDescription);
        //levelDescription.AddRoom(3, corridorRoomDescription);
        //levelDescription.AddRoom(4, roomDescription);
        //levelDescription.AddRoom(5, corridorRoomDescription);
        //levelDescription.AddRoom(6, roomDescription);
        //levelDescription.AddRoom(7, corridorRoomDescription);
        //levelDescription.AddRoom(8, roomDescription);

        //levelDescription.AddConnection(0, 1);
        //levelDescription.AddConnection(1, 2);
        //levelDescription.AddConnection(2, 3);
        //levelDescription.AddConnection(3, 4);
        //levelDescription.AddConnection(4, 5);
        //levelDescription.AddConnection(5, 6);
        //levelDescription.AddConnection(6, 7);
        //levelDescription.AddConnection(7, 8);

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
                PolygonGrid2D.GetRectangle(rnd.Next(4, roomWidth), rnd.Next(4, roomHeight)),
                doors,
                allowedTransformations: transformations);

            roomsTemplates.Add(roomTemplate);
        }

        var squareRoomTemplate = new RoomTemplateGrid2D(
            PolygonGrid2D.GetSquare(8),
            doors
        );

        roomsTemplates.Add(squareRoomTemplate);
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
        var levelDescription = GetLevelDescription();
        var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
        var layout = generator.GenerateLayout();

        return layout;

    }
}
