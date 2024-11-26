using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GladiatorsFight
{
    static class UserUtills
    {
        private static Random s_random = new Random();

        public static string GetSpacebar()
        {
            return " ";
        }

        public static string GetSeparator()
        {
            return " | ";
        }

        public static int ReturnRandomValue(int leftBound, int rightBound)
        {
            return s_random.Next(leftBound, rightBound);
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
        private bool _tryAgain;

        public Arena()
        {
            FillFightersList();
        }

        private void FillFightersList()
        {
            int highHealth = 150;
            int midHealth = 120;
            int lowHealth = 90;

            int highDamage = 15;
            int midDamage = 10;
            int lowDamage = 5;

            _fighters.Add(new Fighter1(midHealth, midDamage));
            _fighters.Add(new Fighter2(highHealth, midDamage));
            _fighters.Add(new Fighter3(midHealth, midDamage));
            _fighters.Add(new Fighter4(lowHealth, lowDamage));
            _fighters.Add(new Fighter5(midHealth, highDamage));
        }

        public void Execute()
        {
            do
            {
                RunMenu();
                StartFighting();
                _tryAgain = IsRestart();
                Console.Clear();
            }
            while (_tryAgain);

            Console.WriteLine("Конец.");
        }

        private bool IsRestart()
        {
            ConsoleKey exitKey = ConsoleKey.Escape;
            Console.WriteLine(exitKey + " - нажмите для выхода. Остальные клавиши для повторного боя.");

            return Console.ReadKey(false).Key != exitKey;
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

            foreach (Fighter fighter in _fighters)
            {
                Console.Write(count + UserUtills.GetSeparator());
                fighter.ShowInfo();
                count++;
            }
        }

        private void StartFighting()
        {
            Fight _fight = new Fight(_fighters[_leftFighterIndex].ReturnNewFighter(), _fighters[_rightFighterIndex].ReturnNewFighter());

            _fight.Execute();
        }
    }

    class Fight
    {
        private const int LeftFighterIndex = 0;
        private const int RightFighterIndex = 1;

        private Fighter _winner;
        private Fighter[] _fighters;


        public Fight(Fighter leftFighter, Fighter rightFighter)
        {
            _fighters = new Fighter[]
            {
                leftFighter, rightFighter
            };
        }

        public void Execute()
        {
            if (IsTheSameName())
            {
                Rename();
            }

            while (_fighters[LeftFighterIndex].IsAlive && _fighters[RightFighterIndex].IsAlive)
            {
                _fighters[LeftFighterIndex].Attack(_fighters[RightFighterIndex]);
                _fighters[RightFighterIndex].Attack(_fighters[LeftFighterIndex]);
            }

            if (TryGetWinner(out _winner))
            {
                Console.WriteLine($"Есть победитель {_winner.Name}");
            }
            else
            {
                Console.WriteLine("Оба померли...");
            }
        }

        private bool IsTheSameName()
        {
            return _fighters[LeftFighterIndex].Name == _fighters[RightFighterIndex].Name;
        }

        private void Rename()
        {
            for (int i = 0; i < _fighters.Length; i++)
            {
                switch (i)
                {
                    case LeftFighterIndex:
                        _fighters[i].AddPositionName("Левый");
                        break;

                    case RightFighterIndex:
                        _fighters[i].AddPositionName("Правый");
                        break;
                }
            }
        }

        private bool TryGetWinner(out Fighter winner)
        {
            bool hasWinner = true;

            if (_fighters[LeftFighterIndex].IsAlive)
            {
                winner = _fighters[LeftFighterIndex];
            }
            else if (_fighters[RightFighterIndex].IsAlive)
            {
                winner = _fighters[RightFighterIndex];
            }
            else
            {
                hasWinner = false;
                winner = null;
            }

            return hasWinner;
        }
    }

    interface IDamageable
    {
        void TakeDamage(int damage);
    }

    abstract class Fighter : IDamageable
    {
        protected int MaxHealth;

        protected int Damage;
        protected int CurrentHealth;

        public Fighter(int maxHealth, int damage)
        {
            MaxHealth = maxHealth;
            Damage = damage;
            CurrentHealth = MaxHealth;
        }

        protected internal string Name { get; protected set; }
        protected internal bool IsAlive => CurrentHealth > 0;

        public void AddPositionName(string addedName)
        {
            Name += UserUtills.GetSpacebar() + addedName;
        }

        public virtual void TakeDamage(int damage)
        {
            Console.WriteLine($"{Name} полуает {damage} урона.");
            CurrentHealth -= damage;
        }

        public abstract Fighter ReturnNewFighter();

        public virtual void Attack(IDamageable target)
        {
            Console.Write($"{Name} начинает атаку");
        }

        protected abstract bool TryApplySpecialAction();

        protected internal virtual void ShowInfo()
        {
            Console.Write($"Имя - {Name}{UserUtills.GetSeparator()}Здоровье - {MaxHealth}{UserUtills.GetSeparator()}Урон - {Damage}{UserUtills.GetSeparator()}");
        }
    }

    class Fighter1 : Fighter
    {
        private int _criticalChance = 30;

        public Fighter1(int health, int damage) : base(health, damage)
        {
            Name = "Критовик";
        }

        public override void Attack(IDamageable target)
        {
            base.Attack(target);

            target.TakeDamage(Damage * ReturnCriticalFactor());
        }

        private int ReturnCriticalFactor()
        {
            int minCriticalFactor = 1;
            int maxCriticalFactor = 2;

            if (TryApplySpecialAction())
            {
                Console.WriteLine("ДА! Крит получается! ");
                return maxCriticalFactor;
            }
            else
            {
                Console.WriteLine("Нет, увы крит не прошёл. ");
                return minCriticalFactor;
            }
        }

        protected override bool TryApplySpecialAction()
        {
            int minChance = 0;
            int maxChance = 101;
            Console.Write(" пробует крит иии...");

            return UserUtills.ReturnRandomValue(minChance, maxChance) <= _criticalChance;
        }

        public override Fighter ReturnNewFighter()
        {
            return new Fighter1(this.MaxHealth, this.Damage);
        }

        protected internal override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine(_criticalChance + " процентов, что влупит двойной урон!");
        }

    }

    class Fighter2 : Fighter
    {
        private int _turnsCount = 1;
        private int _doubleAttackCount = 3;
        private int _currentAttackPerTurn = 1;

        public Fighter2(int health, int damage) : base(health, damage)
        {
            Name = "Двуручник";
        }

        public override void Attack(IDamageable target)
        {
            base.Attack(target);

            int baseAttackPerTurn = 1;
            int maxAttackPerTurn = 2;

            if (TryApplySpecialAction())
            {
                Console.WriteLine(" Пришла пора жахнуть дважды!");
                _currentAttackPerTurn = maxAttackPerTurn;
            }
            else
            {
                Console.WriteLine(" В этот раз один удар.");
                _currentAttackPerTurn = baseAttackPerTurn;
            }

            for (int i = 0; i < _currentAttackPerTurn; i++)
            {
                target.TakeDamage(Damage);
            }
        }
        protected override bool TryApplySpecialAction()
        {
            Console.Write(" пробует второй удар иии...");

            return _turnsCount % _doubleAttackCount == 0;
        }

        protected internal override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Каждую {_doubleAttackCount} атаку бьёт дважды!");
        }

        public override Fighter ReturnNewFighter()
        {
            return new Fighter2(this.MaxHealth, this.Damage);
        }
    }

    class Fighter3 : Fighter
    {
        private int _currentRage = 0;
        private int _rageByDamage = 5;
        private int _rageToHeal = 15;

        public Fighter3(int health, int damage) : base(health, damage)
        {
            Name = "Пожирающий ярость";
        }

        public override void Attack(IDamageable target)
        {
            base.Attack(target);

            if (TryApplySpecialAction())
            {
                Heal();
            }
            else
            {
                Console.WriteLine("Нет, ярости мало!");
            }

            target.TakeDamage(Damage);
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);

            AddRage();
        }

        private void AddRage()
        {
            Console.WriteLine(Name + " копит ярость.");
            _currentRage += _rageByDamage;
        }

        private void Heal()
        {
            Console.WriteLine("Да лечится!");
            CurrentHealth += _rageToHeal;

            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }

            _currentRage = 0;
        }

        protected override bool TryApplySpecialAction()
        {
            Console.Write(" пытается преобразовать ярость в здоровье и...");
            return _currentRage == _rageToHeal;
        }

        protected internal override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"За удар копит {_rageByDamage} ярости, накопив {_rageToHeal} - преобразует её в здоровье.");
        }

        public override Fighter ReturnNewFighter()
        {
            return new Fighter3(this.MaxHealth, this.Damage);
        }
    }

    class Fighter4 : Fighter
    {
        private int _mana = 15;
        private int _manaPriceFireball = 8;
        private int _manaRegeneration = 4;
        private int _fireballDamage = 25;

        public Fighter4(int health, int damage) : base(health, damage)
        {
            Name = "Файеболлер";
        }

        public override void Attack(IDamageable target)
        {
            base.Attack(target);

            int currenDamage = Damage;

            if (TryApplySpecialAction())
            {
                currenDamage = _fireballDamage;
                SpendMana();
            }
            else
            {
                AccumulateMana();
            }

            target.TakeDamage(currenDamage);
        }

        private void AccumulateMana()
        {
            Console.WriteLine("Нет! Просто копит ману.");
            _mana += _manaRegeneration;
        }

        private void SpendMana()
        {
            Console.WriteLine("Да! У него получается.");
            _mana -= _manaPriceFireball;
        }

        protected override bool TryApplySpecialAction()
        {
            Console.Write(" Пробует запустить файерболл и...");

            return _mana >= _manaPriceFireball;
        }

        protected internal override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Есть {_mana} маны. Файерболл наносит {_fireballDamage} урона, а стоит {_manaPriceFireball} маны. Накопление маны {_manaRegeneration} за ход.");
        }

        public override Fighter ReturnNewFighter()
        {
            return new Fighter4(this.MaxHealth, this.Damage);
        }

    }

    class Fighter5 : Fighter
    {
        private int _dodgeChance = 33;

        public Fighter5(int health, int damage) : base(health, damage)
        {
            Name = "Уворотчик";
        }

        public override void Attack(IDamageable target)
        {
            base.Attack(target);

            target.TakeDamage(Damage);
        }

        public override Fighter ReturnNewFighter()
        {
            return new Fighter5(this.MaxHealth, this.Damage);
        }

        public override void TakeDamage(int damage)
        {
            if (TryApplySpecialAction())
            {
                Console.WriteLine(Name + "Увернулся!");
            }
            else
            {
                base.TakeDamage(damage);
            }
        }

        protected override bool TryApplySpecialAction()
        {
            int maxChance = 101;

            return UserUtills.ReturnRandomValue(0, maxChance) >= _dodgeChance;
        }

        protected internal override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Имеет {_dodgeChance} процентный шанс уворота от любой атаки/заклинания.");
        }
    }
}

