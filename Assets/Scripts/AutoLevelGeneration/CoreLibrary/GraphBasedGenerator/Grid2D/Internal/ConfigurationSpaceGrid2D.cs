﻿using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.ConfigurationSpaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.Utils;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal
{
    public class ConfigurationSpaceGrid2D : IConfigurationSpace<Vector2Int>
    {
        private readonly List<OrthogonalLineGrid2D> lines;

        public IReadOnlyList<OrthogonalLineGrid2D> Lines { get; }

        private readonly List<Tuple<OrthogonalLineGrid2D, DoorLineGrid2D>> reverseDoors;

        public IReadOnlyList<Tuple<OrthogonalLineGrid2D, DoorLineGrid2D>> ReverseDoors { get; }

        public ConfigurationSpaceGrid2D(List<OrthogonalLineGrid2D> lines, List<Tuple<OrthogonalLineGrid2D, DoorLineGrid2D>> reverseDoors = null)
        {
            this.lines = lines;
            this.reverseDoors = reverseDoors;
            Lines = lines.AsReadOnly();

            if (reverseDoors != null)
            {
                this.reverseDoors = reverseDoors;
                ReverseDoors = reverseDoors.AsReadOnly();
            }
        }

        public Vector2Int GetRandomPosition(Random random)
        {
            var line = lines.GetWeightedRandom(x => x.Length + 1, random);
            return line.GetNthPoint(random.Next(line.Length + 1));
        }

        public IEnumerable<Vector2Int> ShuffleAndSamplePositions(int maxPointsPerLine, Random random)
        {
            var intersection = lines.ToList();
            intersection.Shuffle(random);

            foreach (var intersectionLine in intersection)
            {
                if (intersectionLine.Length > maxPointsPerLine)
                {
                    // TODO: this is quite bad when mod == 1
                    var mod = intersectionLine.Length / maxPointsPerLine;

                    for (var i = 0; i < maxPointsPerLine; i++)
                    {
                        var position = intersectionLine.GetNthPoint(i != maxPointsPerLine - 1 ? i * mod : intersectionLine.Length);
                        yield return position;
                    }
                }
                else
                {
                    var points = intersectionLine.GetPoints();
                    points.Shuffle(random);

                    foreach (var position in points)
                    {
                        yield return position;
                    }
                }
            }
        }
    }
}