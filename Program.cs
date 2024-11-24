using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiatorsFight
{
    static class UserUtills
    {
        private static Random s_random = new Random();

        public static int ReturnRandomChance()
        {
            int maxChance = 100;

            return s_random.Next(maxChance);
        }

        public static int GetIndexFromUserInput(int maxValue)
        {
            int userInput;

            do
            {
                Console.Write("Введите число: ");
            }
            while (int.TryParse(Console.ReadLine(), out userInput) == false || userInput <= 0 || userInput > maxValue);

            return userInput - 1;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Arena arena = new Arena();
            arena.Execute();
        }
    }

    class Arena
    {
        private List<Fighter> _fighters = new List<Fighter>();
        private int _leftFighterIndex;
        private int _rightFighterIndex;
        private Fight _fight;

        public void Execute()
        {
            RunMenu();

        }

        private void RunMenu()
        {
            Console.WriteLine("Приветствуем вас на Арене нашего безопасного колизея!" +
                "\nВ бою не пострадает ни одно живое существо - все бойцы смоделированы по каталогу.");
            ShowFightersLibrary();
            Console.WriteLine("Боец слева ходит первым, кто же это, под каким номером?");
            _leftFighterIndex = UserUtills.GetIndexFromUserInput(_fighters.Count);
            Console.WriteLine("Ждём номера бойца  справа, ему выпадет второй ход!");
            _rightFighterIndex = UserUtills.GetIndexFromUserInput(_fighters.Count);
        }

        private void ShowFightersLibrary()
        {
            int count = 1;
            string separator = " ";
            foreach (Fighter fighter in _fighters)
            {
                Console.Write(count + separator);
                fighter.ShowInfo();
            }
        }

        private void StartFighting()
        {
            _fight = new Fight(_fighters[_leftFighterIndex].ReturnNewFighter(), _fighters[_rightFighterIndex].ReturnNewFighter());

            
        }
    }

    class Fight
    {
        private Fighter _leftFighter;
        private Fighter _rightFighter;

        public Fight(Fighter leftFighter, Fighter rightFighter)
        {
            _leftFighter = leftFighter;
            _rightFighter = rightFighter;
        }

        public void Began()
        {
            _leftFighter.MakeTurn(_rightFighter);
            _rightFighter.MakeTurn(_leftFighter);
        }
    }

    interface ITurnable
    {
        void MakeTurn(IDamageable target);
    }

    interface IDamageable
    {
        void TakeDamage(int damage);
    }

    class Fighter : IDamageable, ITurnable
    {
        protected int BaseDamage = 10;
        protected int MaxHealth = 100;
        protected int AttackCountPerRound = 1;

        public virtual void ShowInfo()
        {

        }

        public virtual void TakeDamage(int damage)
        {
            MaxHealth -= damage;
        }

        public Fighter ReturnNewFighter()
        {
            return new Fighter();
        }

        public virtual void MakeTurn(IDamageable target)
        {

        }

        protected internal virtual int ReturnFinalDamage()
        {
            return BaseDamage;
        }
    }

    class Fighter1 : Fighter
    {
        private int _criticalChance = 30;
        private int _criticalDamageFactor = 2;

        protected internal override int ReturnFinalDamage()
        {
            int finalDamage = BaseDamage;

            if (UserUtills.ReturnRandomChance() <= _criticalChance)
            {
                finalDamage *= _criticalDamageFactor;
            }

            return finalDamage;
        }
    }

    class Fighter2 : Fighter
    {
        private int _attackCount = 1;
        private int _doubleAttackCount = 3;
        private int _currentAttackCount;

        protected override int ReturnFinalDamage()
        {
            int finalDamage = BaseDamage;
            _currentAttackCount = AttackCountPerRound;

            if (_attackCount % _doubleAttackCount == 0)
            {
                AttackCountPerRound++;
            }

            return finalDamage;
        }
    }

    class Fighter3 : Fighter
    {
        private int _currentHealth;

    }
}

