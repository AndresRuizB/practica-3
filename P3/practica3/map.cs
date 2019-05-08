using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace practica3
{

    // posibles direcciones
    public enum Direction { North, South, East, West };
    class map
    {
        Dungeon[] dungeons; // vector de lugares del mapa
        Enemy[] enemies; // vector de enemigos del juego
        //int nDungeons, nEnemies; // numero de lugares y numero de enemigos del mapa

        struct Enemy
        {
            public string name, description;
            public int attHP, attATK;
        }

        // lugares del mapa
        struct Dungeon
        {
            public string name, description;
            public bool exit; // es salida?
            public int[] doors; // vector de 4 componentes con el lugar
                                // al norte, sur, este y oeste respectivamente
                                // -1 si no hay conexion
            public Lista enemiesInDungeon; // lista de enteros, indices al vector de enemigos
        }

        public map(int numDungeons, int numEnem)  //constructora de la clase vacia
        {
            enemies = new Enemy[numEnem];
            dungeons = new Dungeon[numDungeons];

            for (int i = 0; i < dungeons.Length; i++)
            {
                dungeons[i].doors = new int[] { -1, -1, -1, -1 };  //establece todas las puertas a "no conexion"
            }
        }


        public void ReadMap(string file)
        {
            StreamReader entry = new StreamReader(file);

            string line;
            string[] lineDiv = new string[7];

            while (!entry.EndOfStream)
            {
                line = entry.ReadLine();
                lineDiv = line.Split(' ');

                if (lineDiv[0] == "dungeon")
                {
                    dungeons[int.Parse(lineDiv[1])] = CreateDungeon(lineDiv, entry);
                }
                else if (lineDiv[0] == "door")
                {
                    CreateDoor(lineDiv, ref dungeons);
                }
                else if (lineDiv[0] == "enemy")
                {
                    enemies[int.Parse(lineDiv[1])] = CreateEnemy(lineDiv);
                }

            }

            entry.Close();
        }

        Dungeon CreateDungeon(string[] lineDiv, StreamReader entry)
        {
            Dungeon dungeon = new Dungeon { };

            dungeon.name = lineDiv[2];
            dungeon.exit = lineDiv[3].Equals("exit");
            dungeon.doors = new int[4];

            for (int i = 0; i < 4; i++)
            {
                dungeon.doors[i] = -1;
            }

            dungeon.enemiesInDungeon = new Lista();

            dungeon.description = ReadDescription(entry);

            return dungeon;
        }

        private string ReadDescription(StreamReader f)
        {

            string desLine = "", desTotal = "";

            do
            {
                desLine = f.ReadLine();
                desTotal = desTotal + "\n" + desLine;
            } while (desLine != "" && desLine != " ");

            return desTotal;
        }

        void CreateDoor(string[] lineDiv, ref Dungeon[] dungeons)
        {
            //  0      1                   2         3                           4             5         6
            //  door   [nº de la puerta]   dungeon   [de donde sale la puerta]   [direccion]   dungeon   [a donde llega la puerta]
            //  door   0                   dungeon   0                           north         dungeon   3

            switch (lineDiv[4])
            {                                                           //0 north,  1 south,  2 east,  3 west
                case "north":
                    dungeons[int.Parse(lineDiv[3])].doors[0] = int.Parse(lineDiv[6]);
                    dungeons[int.Parse(lineDiv[6])].doors[1] = int.Parse(lineDiv[3]);   //conectamos tambien al reves
                    break;
                case "south":
                    dungeons[int.Parse(lineDiv[3])].doors[1] = int.Parse(lineDiv[6]);
                    dungeons[int.Parse(lineDiv[6])].doors[0] = int.Parse(lineDiv[3]);
                    break;
                case "east":
                    dungeons[int.Parse(lineDiv[3])].doors[2] = int.Parse(lineDiv[6]);
                    dungeons[int.Parse(lineDiv[6])].doors[3] = int.Parse(lineDiv[3]);
                    break;
                case "west":
                    dungeons[int.Parse(lineDiv[3])].doors[3] = int.Parse(lineDiv[6]);
                    dungeons[int.Parse(lineDiv[6])].doors[2] = int.Parse(lineDiv[3]);
                    break;
            }
        }

        Enemy CreateEnemy(string[] lineDiv)
        {
            Enemy enemy = new Enemy { };

            enemy.name = "Enemy" + lineDiv[1];
            enemy.description = lineDiv[2];
            enemy.attATK = int.Parse(lineDiv[4]);
            enemy.attHP = int.Parse(lineDiv[3]);

            dungeons[int.Parse(lineDiv[6])].enemiesInDungeon.insertaFin(int.Parse(lineDiv[1]));

            return enemy;
        }


        public string GetDungeonInfo(int dung)
        {
            string info;

            info = "Dungeon: " + dungeons[dung].name + "\n" + dungeons[dung].description; //recauda la informacion basica de la dungeon

            if (dungeons[dung].exit) info += "Exit: true"; //añada si tiene o no salida
            else info += "Exit: false";

            return info;
        }

        public string GetMoves(int dung)
        {
            string moves = "";

            for (int i = 0; i < 4; i++)
            {
                int proximadun = dungeons[dung].doors[i];

                if (proximadun != -1)
                {
                    if (i == 0)  //norte
                    {
                        moves += "north: " + dungeons[proximadun].name + "\n";
                    }
                    else if (i == 1)  //sur
                    {
                        moves += "south: " + dungeons[proximadun].name + "\n";
                    }
                    else if (i == 2)  //este
                    {
                        moves += "east: " + dungeons[proximadun].name + "\n";
                    }
                    else if (i == 3)  //oeste
                    {
                        moves += "west: " + dungeons[proximadun].name + "\n";
                    }
                }

            }

            return moves;  //todas los posibles movimientos que tiene el jugador a su disposicion
        }

        public int GetNumEnemies(int dung)
        {
            int numeroDeEnemigos = 0;
            if (dungeons[dung].enemiesInDungeon != null) numeroDeEnemigos = dungeons[dung].enemiesInDungeon.cuentaEltos(); //cuenta los elementos enlazados de la lista de enemigos

            return numeroDeEnemigos;
        }

        private string GetEnemyInfo(int en)
        {
            string info = en + ": ";

            Enemy enemigo = enemies[en];
            info += enemigo.name + " HP " + enemigo.attHP + " ATK " + enemigo.attATK; //recauda la informacio sobre los enemigos

            return info;
        }

        public string GetEnemiesInfo(int dung)
        {
            string info = "";
            int numeroDeEnemigos = GetNumEnemies(dung);

            if (numeroDeEnemigos > 0)
            {
                for (int i = 0; i < numeroDeEnemigos; i++)
                {
                    int enemigo = dungeons[dung].enemiesInDungeon.nEsimo(i);

                    info += GetEnemyInfo(enemigo);
                    info += "\n";
                } //junta la informacion de todos los enemigos en una dungeon
            }
            else info = "No hay enemigos";



            return info;
        }

        public int GetEnemyATK(int en)
        {
            return enemies[en].attATK;  //devuelve los puntos de ataque de cierto enemigo
        }

        private bool MakeDamageEnemy(int en, int damage)
        {
            enemies[en].attHP -= damage; //resta el daño al enemigo
            int hpRestante = enemies[en].attHP;

            if (hpRestante <= 0) return true; //si sigue vivo
            else return false; //si se ha muerto
        }

        public int AttackEnemiesInDungeon(int dung, int damage)
        {
            int numeroDeEnemigos = GetNumEnemies(dung);
            bool enemigoMuerto;
            int numeroDeEnemigosMuertos = 0;

            for (int i = 0; i < numeroDeEnemigos; i++)
            {
                int enemigo = dungeons[dung].enemiesInDungeon.nEsimo(i);
                enemigoMuerto = MakeDamageEnemy(enemigo, damage); //hace daño al enemigo

                if (enemigoMuerto)
                {
                    dungeons[dung].enemiesInDungeon.borraElto(i); //elimina al enemigo de la lista
                    numeroDeEnemigosMuertos++; //aumenta el numero de enemigos muertos
                    numeroDeEnemigos--; //enemigos restantes
                    i--;
                }
            } //para todos los enemigos en la dungeon

            return numeroDeEnemigosMuertos;
        }

        public int ComputeDungeonDamage(int dung)
        {
            int numeroDeEnemigos = GetNumEnemies(dung);
            int sumaDeATK = 0;

            for (int i = 0; i < numeroDeEnemigos; i++)
            {
                sumaDeATK += enemies[dungeons[dung].enemiesInDungeon.nEsimo(i)].attATK;
            } //suma los puntos de ataque de todos los enemigos que hay en una dungeon

            return sumaDeATK;
        }

        public int Move(int pl, Direction dir)
        {
            int proximoLugar = -1;

            if (dungeons[pl].doors[(int)dir] != -1) {
                proximoLugar = dungeons[pl].doors[(int)dir];
            }

            return proximoLugar;
        }

        public bool isExit(int dung)
        {
            return dungeons[dung].exit; //devuelve true si esa dungeon tiene salida
        }

    }
}
