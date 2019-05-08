using System;
using System.Collections.Generic;
using System.Text;

namespace practica3
{
    class player
    {
        int pos;  //posicion del jugador en el mapa
        int health, damage;

        public player()
        {
            pos = 0;
            health = 10;
            damage = 2;
        }  //falta q sean ocnstantes

        public int GetPosition()
        {
            return pos;
        }  //devuelve posicion del jugador

        public bool IsAlive()
        {
            if (health > 0) return true;
            else return false;
        }  //devuelve true si esta vivo

        public string PrintStatus()
        {
            string info = "Player: ";
            info += "HP " + health + " ATK " + damage;
            return info;
        }  //devuelve el HP y ATK del jugador

        public string GetATK()
        {
            return damage.ToString();
        }  //devuelve los puntos de atque del jugador

        public bool ReciveDamage(int dam)
        {
            health -= dam;  //recibe el daño

            if (health > 0) return true;
            else return false;
        }  //devuelve true si sigue vivo tras recibir el daño

        public void Move(map m, Direction dir)
        {
            if (m.Move(pos, dir) != -1)
            {
                pos = m.Move(pos, dir);
            }
            else Console.WriteLine("No hay puerta en esa direccion\n");

        }  //mueve al jugador usando el mapa

        public bool atExit(map mapa)
        {
            bool exit = mapa.isExit(pos);  //mira si esa sala tiene salida
            return exit;
        }  //devuleve true si la sala en la que se encuantra al jugador tiene salida
    }
}
