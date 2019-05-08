using System;
using System.IO;

namespace practica3
{
    class Program
    {
        static void Main(string[] args)
        {
            map mapa = new map(10, 3); //inicializa el mapa

            mapa.ReadMap("HauntedHous.map");  //lee el mapa de archivo

            player ply = new player();

            bool jugando = true;
            string[] comandos = { "go north", "go east", "go west", "go south", "enemies", "attack", "info", "status", "quit" };

            Lista mem = new Lista();

            Console.WriteLine("Que deseas hacer? Cargar partida de archivo(1) o jugar(2)");
            int resp = int.Parse(Console.ReadLine());

            if (resp == 1)
            {
                Lectura(ply, mapa, ref jugando, mem);
            }
            else
            {
                Console.Clear();
                Console.WriteLine(mapa.GetDungeonInfo(ply.GetPosition()) + "\n");  //muestra la informacion de la dungeon en la que se encuantra el jugador
            }
            

            //Lectura(ply, mapa, ref jugando, ref mem);

            while (jugando)
            {

                Console.Write("> ");
                string comando = Console.ReadLine();
                comando = comando.ToLower();

                while (!ComandoValido(comandos, comando))
                {
                    Console.WriteLine("Comando no valido");
                    Console.Write("> ");
                    comando = Console.ReadLine();
                }  //en el caso de recibir un comando no valido 


                if (comando == "quit")
                {
                    jugando = false;  //el jugador sale del juego
                    GrabaPartida(mem, "memory");
                }


                else
                {
                    ProcesaInput(comando, ply, mapa, mem);  //procesa el input

                    if (!ply.IsAlive())  //si has muerto tras el ataque
                    {
                        Console.Clear();
                        Console.WriteLine("Has muerto");
                        jugando = false;
                    }
                    else if (ply.atExit(mapa))  //si has llegado a una salida
                    {
                        Console.Clear();
                        jugando = false;
                        Console.WriteLine("Enhorabuena, has encontrado una salida");  //si llegas a una salida se termina el juego
                    }
                }


            } //bucle de juago
        }

        static void ProcesaInput(string com, player p, map m, Lista mem)
        {
            if (com == "go north")
            {
                Console.Clear();
                p.Move(m, Direction.North); //mueve al jugador          
                EnemiesAttackPlayer(m, p);  //si hay enemigos en la dungeon donde se encuentra el jugador, estos le atacan
                Console.WriteLine(m.GetDungeonInfo(p.GetPosition()) + "\n");  //muestra la informacion de la dungeon en la que se encuantra el jugador
            } //norte
            else if (com == "go east")
            {
                Console.Clear();
                p.Move(m, Direction.East);//mueve al jugador   
                EnemiesAttackPlayer(m, p);  //si hay enemigos en la dungeon donde se encuentra el jugador, estos le atacan
                Console.WriteLine(m.GetDungeonInfo(p.GetPosition()) + "\n");  //muestra la informacion de la dungeon en la que se encuantra el jugador
            } //este
            else if (com == "go south")
            {
                Console.Clear();
                p.Move(m, Direction.South);//mueve al jugador   
                EnemiesAttackPlayer(m, p);  //si hay enemigos en la dungeon donde se encuentra el jugador, estos le atacan
                Console.WriteLine(m.GetDungeonInfo(p.GetPosition()) + "\n");  //muestra la informacion de la dungeon en la que se encuantra el jugador
            } //sur
            else if (com == "go west")
            {
                Console.Clear();
                p.Move(m, Direction.West);//mueve al jugador   
                EnemiesAttackPlayer(m, p);  //si hay enemigos en la dungeon donde se encuentra el jugador, estos le atacan
                Console.WriteLine(m.GetDungeonInfo(p.GetPosition()) + "\n");  //muestra la informacion de la dungeon en la que se encuantra el jugador
            } //oeste
            else if (com == "enemies")
            {
                Console.WriteLine(m.GetEnemiesInfo(p.GetPosition()) + "\n");  //mustra la informacion de todos los enemigos que hay en la dungeon donde se encuantra el jugador
            }
            else if (com == "attack")
            {
                Console.WriteLine("Numero de enemigos eliminados " + PlayerAttackEnemies(m, p) + "\n"); //ataca a los enemigos que se encuantran en la dungeon
                EnemiesAttackPlayer(m, p);  //si hay enemigos en la dungeon donde se encuentra el jugador, estos le atacan
            } //atacar
            else if (com == "status")
            {
                Console.WriteLine(p.PrintStatus() + "\n");  //imprime status del jugador 
            }
            else if (com == "info")
            {
                Console.WriteLine(m.GetDungeonInfo(p.GetPosition()) + "\n");  //muestra la informacion de la dungeon en la que se encuantra el jugador
                Console.WriteLine(m.GetMoves(p.GetPosition()) + "\n");  //muestra las posibles direcciones en las que se pueden mover el jugador
            }

            if (com.Contains("go") || com.Contains("attack"))
            {
                GuardaProgreso( mem, com);
            }

        }

        static bool EnemiesAttackPlayer(map m, player p)
        {
            int numeroDeEnemigos = m.GetNumEnemies(p.GetPosition()); //cuenta el numero de enemigos que hay en la dungeon donde se encuantra el jugador

            if (numeroDeEnemigos > 0)
            {
                int dañoDeEnemigos = m.ComputeDungeonDamage(p.GetPosition()); //suma los puntos de ataque de todos los enemigos de la dungeon
                p.ReciveDamage(dañoDeEnemigos); //el jugador recibe el daño
                return true;  //si hay enemigos
            }
            else return false;
        }

        static int PlayerAttackEnemies(map m, player p)
        {
            return m.AttackEnemiesInDungeon(p.GetPosition(), Int32.Parse(p.GetATK()));  //devuelve el numero de enemigos que ha matado el jugador
        }

        static bool ComandoValido(string[] comandos, string comando) //devuelve true si el comando que le llega es computable
        {
            int i = 0;
            bool encontrado = false;

            while (!encontrado && i < comandos.Length)
            {
                if (comandos[i] == comando) encontrado = true;
                i++;
            }
            return encontrado;
        }

        static void GuardaProgreso(Lista mem, string com)
        {
            switch (com)
            {
                case "go north":
                    mem.insertaFin(0);
                    break;
                case "go south":
                    mem.insertaFin(1);
                    break;
                case "go east":
                    mem.insertaFin(2);
                    break;
                case "go west":
                    mem.insertaFin(3);
                    break;
                case "attack":
                    mem.insertaFin(-1);
                    break;
            }
        }

        static void GrabaPartida(Lista mem, string archivo)
        {
            StreamWriter salida = new StreamWriter(archivo + ".txt");

            for (int i = 0; i < mem.cuentaEltos(); i++)
            {
                switch (mem.nEsimo(i))
                {
                    case 0:
                        salida.WriteLine("go north");
                        break;
                    case 1:
                        salida.WriteLine("go south");
                        break;
                    case 2:
                        salida.WriteLine("go east");
                        break;
                    case 3:
                        salida.WriteLine("go west");
                        break;
                    case -1:
                        salida.WriteLine("attack");
                        break;

                }
            }

            salida.Close();
        }

        static void Lectura(player p, map m, ref bool jugando, Lista mem)
        {
            StreamReader entry = new StreamReader("memory.txt");
            string linea;

            while (!entry.EndOfStream)
            {
                linea = entry.ReadLine(); //lee cada comando
                ProcesaInput(linea, p, m, mem);
            }
            if (p.atExit(m))  //si has llegado a una salida
            {
                Console.WriteLine("Enhorabuena, has encontrado una salida");  //si llegas a una salida se termina el juego
                jugando = false;
            }

            entry.Close();
        }
    }
}
