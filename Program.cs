using Raylib_cs;
using System.Numerics;

namespace LightingTesting
{
    internal class Program
    {

        struct LightData
        {

            public Vector2 LightPosition;
            public byte LightValue;

            public LightData(Vector2 lightPosition, byte lightValue)
            {

                LightPosition = lightPosition;
                LightValue = lightValue;

            }

        }
        static void Main(string[] args)
        {

            int worldSize = 256;
            int tileSize = 32;
            float scalingFactor = 1;
            byte selectorLightValue = 15;
            float offsetX = 0;
            float offsetY = 0;
            int padding = 0;
            byte[,] lightingData = new byte[worldSize,worldSize];
            bool[,] solidMask = new bool[worldSize,worldSize];
            Queue<LightData> lightAdditionQueue = new();
            Queue<LightData> lightRemovalQueue = new();

            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
            Raylib.InitWindow(800, 600, "Hello World");

            while (!Raylib.WindowShouldClose())
            {

                while (lightAdditionQueue.Count > 0)
                {

                    if (lightAdditionQueue.TryDequeue(out LightData lightParameters) && solidMask[(int)lightParameters.LightPosition.X, (int)lightParameters.LightPosition.Y] == false)
                    {

                        byte startingLightValue = lightParameters.LightValue;
                        List<Vector2> samplePositions = [lightParameters.LightPosition];

                        while (startingLightValue > 0)
                        {

                            List<Vector2> newSamples = new();
                            foreach (Vector2 sample in samplePositions)
                            {

                                // if (lightingData[(int)sample.X, (int)sample.Y] < startingLightValue) lightingData[(int)sample.X, (int)sample.Y] = startingLightValue;
                                lightingData[(int)sample.X, (int)sample.Y] = Math.Max(lightingData[(int)sample.X, (int)sample.Y], startingLightValue);

                                if (sample.X + 1 < worldSize && !solidMask[(int)sample.X + 1, (int)sample.Y] && lightingData[(int)sample.X + 1, (int)sample.Y] < startingLightValue) newSamples.Add(sample + new Vector2(1, 0));
                                if (sample.X - 1 >= 0 && !solidMask[(int)sample.X - 1, (int)sample.Y] && lightingData[(int)sample.X - 1, (int)sample.Y] < startingLightValue) newSamples.Add(sample + new Vector2(-1, 0));
                                if (sample.Y + 1 < worldSize && !solidMask[(int)sample.X, (int)sample.Y + 1] && lightingData[(int)sample.X, (int)sample.Y + 1] < startingLightValue) newSamples.Add(sample + new Vector2(0, 1));
                                if (sample.Y - 1 >= 0 && !solidMask[(int)sample.X, (int)sample.Y - 1] && lightingData[(int)sample.X, (int)sample.Y - 1] < startingLightValue) newSamples.Add(sample + new Vector2(0, -1));

                            }

                            samplePositions = newSamples;

                            startingLightValue--;

                        }

                    }

                }

                while (lightRemovalQueue.Count > 0)
                {

                    if (lightRemovalQueue.TryDequeue(out LightData data))
                    {

                        byte intendedLightValue = data.LightValue;
                        List<Vector2> samplePositions = [data.LightPosition];

                        while (intendedLightValue > 0)
                        {

                            List<Vector2> newSamples = new();

                            foreach (Vector2 sample in samplePositions)
                            {

                                lightingData[(int)sample.X, (int)sample.Y] = 0;
                                LightData intendedData = new LightData(sample, (byte)(intendedLightValue));

                                if (sample.X - 1 >= 0 && lightingData[(int)sample.X - 1, (int)sample.Y] != 0)
                                {
                                    if (lightingData[(int)sample.X - 1, (int)sample.Y] < intendedLightValue)
                                    {
                                        newSamples.Add(sample - Vector2.UnitX);
                                    }
                                    else
                                    {
                                        if (lightingData[(int)sample.X - 1, (int)sample.Y] == intendedLightValue)
                                        {
                                            if (!lightAdditionQueue.Contains(new LightData(sample, (byte)(intendedLightValue - 1)))) lightAdditionQueue.Enqueue(new LightData(sample, (byte)(intendedLightValue - 1)));
                                        }
                                        else
                                        {
                                            if (!lightAdditionQueue.Contains(intendedData)) lightAdditionQueue.Enqueue(intendedData);
                                        }
                                    }
                                }
                                if (sample.X + 1 < worldSize && lightingData[(int)sample.X + 1, (int)sample.Y] != 0)
                                {
                                    if (lightingData[(int)sample.X + 1, (int)sample.Y] < intendedLightValue)
                                    {
                                        newSamples.Add(sample + Vector2.UnitX);
                                    }
                                    else
                                    {
                                        if (lightingData[(int)sample.X + 1, (int)sample.Y] == intendedLightValue)
                                        {
                                            if (!lightAdditionQueue.Contains(new LightData(sample, (byte)(intendedLightValue - 1)))) lightAdditionQueue.Enqueue(new LightData(sample, (byte)(intendedLightValue - 1)));
                                        }
                                        else
                                        {
                                            if (!lightAdditionQueue.Contains(intendedData)) lightAdditionQueue.Enqueue(intendedData);
                                        }
                                    }
                                }
                                if (sample.Y - 1 >= 0 && lightingData[(int)sample.X, (int)sample.Y - 1] != 0)
                                {
                                    if (lightingData[(int)sample.X, (int)sample.Y - 1] < intendedLightValue)
                                    {
                                        newSamples.Add(sample - Vector2.UnitY);
                                    }
                                    else
                                    {
                                        if (lightingData[(int)sample.X, (int)sample.Y - 1] == intendedLightValue)
                                        {
                                            if (!lightAdditionQueue.Contains(new LightData(sample, (byte)(intendedLightValue - 1)))) lightAdditionQueue.Enqueue(new LightData(sample, (byte)(intendedLightValue - 1)));
                                        }
                                        else
                                        {
                                            if (!lightAdditionQueue.Contains(intendedData)) lightAdditionQueue.Enqueue(intendedData);
                                        }
                                    }
                                }
                                if (sample.Y + 1 < worldSize && lightingData[(int)sample.X, (int)sample.Y + 1] != 0)
                                {
                                    if (lightingData[(int)sample.X, (int)sample.Y + 1] < intendedLightValue)
                                    {
                                        newSamples.Add(sample + Vector2.UnitY);
                                    }
                                    else
                                    {
                                        if (lightingData[(int)sample.X, (int)sample.Y + 1] == intendedLightValue)
                                        {
                                            if (!lightAdditionQueue.Contains(new LightData(sample, (byte)(intendedLightValue - 1)))) lightAdditionQueue.Enqueue(new LightData(sample, (byte)(intendedLightValue - 1)));
                                        }
                                        else
                                        {
                                            if (!lightAdditionQueue.Contains(intendedData)) lightAdditionQueue.Enqueue(intendedData);
                                        }
                                    }
                                }

                            }

                            samplePositions = newSamples;

                            intendedLightValue--;

                        }

                    }

                }

                if (Raylib.IsKeyDown(KeyboardKey.LeftControl))
                {

                    if (Raylib.IsMouseButtonDown(MouseButton.Left))
                    {

                        Vector2 delta = Raylib.GetMouseDelta();
                        offsetX += delta.X;
                        offsetY += delta.Y;

                    }

                    if (Raylib.IsMouseButtonDown(MouseButton.Right))
                    {

                        scalingFactor += Raylib.GetMouseDelta().Y * 0.001f;

                    }

                    if (Raylib.IsKeyPressed(KeyboardKey.C))
                    {

                        lightingData = new byte[worldSize, worldSize];
                        solidMask = new bool[worldSize, worldSize];
                        lightAdditionQueue = new();

                    }

                }

                selectorLightValue = (byte) Math.Clamp((selectorLightValue - Raylib.GetMouseWheelMoveV().Y), 0, 15);

                Raylib.ClearBackground(Color.Gray);

                Raylib.BeginDrawing();

                for (int x = 0; x < worldSize; x++)
                {

                    for (int y = 0; y < worldSize; y++)
                    {

                        byte lightValue = (byte) (((lightingData[x, y] & 15) / 15.0) * 255);
                        bool hit = false;
                        Color resultColor = new Color(lightValue, lightValue, lightValue);
                        Vector2 mousePosition = Raylib.GetMousePosition();

                        if (solidMask[x, y]) resultColor = Color.Blue;

                        if (mousePosition.X > (x * (tileSize * scalingFactor)) + offsetX && 
                            mousePosition.X < (x * (tileSize * scalingFactor)) + offsetX + (tileSize * scalingFactor) &&
                            mousePosition.Y > (y * (tileSize * scalingFactor)) + offsetY &&
                            mousePosition.Y < (y * (tileSize * scalingFactor)) + offsetY + (tileSize * scalingFactor))
                        {

                            byte lightColor = (byte) ((selectorLightValue / 15.0f) * 255);
                            // resultColor = new Color(lightColor, lightColor, lightColor);
                            hit = true;

                            // Raylib.DrawRectangleLinesEx(new Rectangle())
                            if (Raylib.IsMouseButtonDown(MouseButton.Right) && !Raylib.IsKeyDown(KeyboardKey.LeftControl))
                            {

                                if (Raylib.IsKeyDown(KeyboardKey.LeftShift)) // remove block
                                {

                                    if (solidMask[x,y] != false)
                                    {

                                        solidMask[x, y] = false;

                                        if (x + 1 < worldSize && lightingData[x + 1, y] != 0 && !lightAdditionQueue.Contains(new LightData(new Vector2(x + 1, y), lightingData[x + 1, y]))) lightAdditionQueue.Enqueue(new LightData(new Vector2(x + 1, y), lightingData[x + 1, y]));
                                        if (x - 1 >= 0 && lightingData[x - 1, y] != 0 && !lightAdditionQueue.Contains(new LightData(new Vector2(x - 1, y), lightingData[x - 1, y]))) lightAdditionQueue.Enqueue(new LightData(new Vector2(x - 1, y), lightingData[x - 1, y]));
                                        if (y + 1 < worldSize && lightingData[x, y + 1] != 0 && !lightAdditionQueue.Contains(new LightData(new Vector2(x, y + 1), lightingData[x, y + 1]))) lightAdditionQueue.Enqueue(new LightData(new Vector2(x, y + 1), lightingData[x, y + 1]));
                                        if (y - 1 >= 0 && lightingData[x, y - 1] != 0 && !lightAdditionQueue.Contains(new LightData(new Vector2(x, y - 1), lightingData[x, y - 1]))) lightAdditionQueue.Enqueue(new LightData(new Vector2(x, y - 1), lightingData[x, y - 1]));

                                    }

                                }
                                else // place block
                                {

                                    solidMask[x, y] = true;

                                    if (lightingData[x, y] != 0 && !lightRemovalQueue.Contains(new LightData(new Vector2(x, y), lightingData[x, y]))) lightRemovalQueue.Enqueue(new LightData(new Vector2(x, y), lightingData[x, y]));

                                }

                            }

                            if (Raylib.IsMouseButtonDown(MouseButton.Left) && !Raylib.IsKeyDown(KeyboardKey.LeftControl) && solidMask[x,y] == false)
                            {

                                if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
                                {

                                    if (!lightRemovalQueue.Contains(new LightData(new Vector2(x,y), lightingData[x,y])))
                                    {

                                        lightRemovalQueue.Enqueue(new LightData(new Vector2(x,y), lightingData[x,y]));

                                    }

                                } else
                                {

                                    if (!lightAdditionQueue.Contains(new LightData(new Vector2(x,y), selectorLightValue)) && !solidMask[x,y] && lightingData[x,y] < selectorLightValue)
                                    {

                                        lightAdditionQueue.Enqueue(new LightData(new Vector2(x, y), selectorLightValue));

                                    }

                                }

                            }

                        }

                        Raylib.DrawRectangleRec(new Rectangle(padding + (x * (tileSize * scalingFactor)) + offsetX, padding + (y * (tileSize * scalingFactor)) + offsetY, (tileSize * scalingFactor) - padding, (tileSize * scalingFactor) - padding), resultColor);
                        if (hit) Raylib.DrawRectangleLinesEx(new Rectangle(padding + (x * (tileSize * scalingFactor)) + offsetX, padding + (y * (tileSize * scalingFactor)) + offsetY, (tileSize * scalingFactor) - padding, (tileSize * scalingFactor) - padding), 1, Color.Red);

                    }

                }

                Raylib.DrawTextEx(Raylib.GetFontDefault(), $"Selected light value: {selectorLightValue}", new Vector2(4, 4), 24, 2, Color.White);

                Raylib.EndDrawing();

                if (lightAdditionQueue.Count > 0) Console.WriteLine($"Light add queue count: {lightAdditionQueue.Count}");
                if (lightRemovalQueue.Count > 0) Console.WriteLine($"Light remove queue count: {lightRemovalQueue.Count}");

            }

            Raylib.CloseWindow();

        }

        public static float Distance(Vector2 a, Vector2 b)
        {

            return (float) Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

        }

    }
}
