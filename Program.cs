using System;
using System.Collections.Generic;

namespace GladiatorsFight
{
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
        private Fighter _leftFighter;
        private Fighter _rightFighter;

        public Arena()
        {
            FillFightersList();
        }

        public void Execute()
        {
            do
            {
                RunMenu();
                UserUtills.CreateEmptyLine();
                HoldFight();
            }
            while (IsRestart());

            Console.WriteLine(" Конец.");
        }

        private bool IsRestart()
        {
            ConsoleKey exitKey = ConsoleKey.Escape;
            Console.WriteLine(exitKey + " - нажмите для выхода. Остальные клавиши для повторного боя.");

            return Console.ReadKey(true).Key != exitKey;
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
            _fighters.Add(new Fighter5(lowHealth, highDamage));
        }

        private void RunMenu()
        {
            Console.WriteLine("Приветствуем вас на Арене нашего безопасного колизея!" +
                "\nВ бою не пострадает ни одно живое существо - все бойцы смоделированы по каталогу.");
            UserUtills.CreateEmptyLine();
            ShowFightersLibrary();
            UserUtills.CreateEmptyLine();
            Console.WriteLine("Боец слева ходит первым, кто же это, под каким номером?");
            _leftFighter = _fighters[UserUtills.GetIndexFromUserInput(_fighters.Count)].Clone();
            Console.WriteLine("Ждём номера бойца  справа, ему выпадет второй ход!");
            _rightFighter = _fighters[UserUtills.GetIndexFromUserInput(_fighters.Count)].Clone();
            UserUtills.CreateEmptyLine();
        }

        private void ShowFightersLibrary()
        {
            int count = 1;

            foreach (Fighter fighter in _fighters)
            {
                Console.Write(count + UserUtills.ReturnSeparator());
                fighter.ShowInfo();
                count++;
            }
        }

        private void HoldFight()
        {
            Fight fight = new Fight(_leftFighter, _rightFighter);

            fight.Execute();
        }
    }

    class Fight
    {
        private Fighter _leftFighter;
        private Fighter _rightFighter;
        private Fighter _winner;

        public Fight(Fighter leftFighter, Fighter rightFighter)
        {
            _leftFighter = leftFighter;
            _rightFighter = rightFighter;
        }

        public void Execute()
        {
            if (IsSameNameFighters())
            {
                RenameFighter();
            }

            Console.WriteLine($"Начался бой между {_leftFighter.Name} и {_rightFighter.Name}.");
            UserUtills.CreateEmptyLine();

            while (_leftFighter.IsAlive && _rightFighter.IsAlive)
            {
                _leftFighter.Attack(_rightFighter);
                UserUtills.CreateEmptyLine();
                _rightFighter.Attack(_leftFighter);
                UserUtills.CreateEmptyLine();
            }

            UserUtills.CreateEmptyLine();

            if (TryGetWinner(out _winner))
            {
                Console.WriteLine($"Есть победитель {_winner.Name}");
            }
            else
            {
                Console.WriteLine("Оба померли...");
            }
        }

        private void RenameFighter()
        {
            _leftFighter.AddPositionName("Левый");
            _rightFighter.AddPositionName("Правый");
        }

        private bool IsSameNameFighters()
        {
            return _leftFighter.Name == _rightFighter.Name;
        }

        private bool TryGetWinner(out Fighter winner)
        {
            bool hasWinner = true;

            if (_leftFighter.IsAlive)
            {
                winner = _leftFighter;
            }
            else if (_rightFighter.IsAlive)
            {
                winner = _rightFighter;
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
            Name += UserUtills.ReturnSpacebar() + addedName;
        }

        public abstract Fighter Clone();

        public virtual void TakeDamage(int damage)
        {
            Console.WriteLine($"{Name} полуает {damage} урона.");
            CurrentHealth -= damage;
        }

        public virtual void Attack(IDamageable target)
        {
            Console.Write($"{Name} начинает атаку ");
        }

        protected internal virtual void ShowInfo()
        {
            Console.Write($"Имя - {Name}{UserUtills.ReturnSeparator()}Здоровье - {MaxHealth}{UserUtills.ReturnSeparator()}Урон - {Damage}{UserUtills.ReturnSeparator()}");
        }

        protected abstract bool TryApplySpecialAction();
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

        public override Fighter Clone()
        {
            return new Fighter1(MaxHealth, Damage);
        }

        protected internal override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine(_criticalChance + " процентов, что влупит двойной урон!");
        }

        protected override bool TryApplySpecialAction()
        {
            int minChance = 0;
            int maxChance = 101;
            Console.Write(" пробует крит иии...");

            return UserUtills.ReturnRandomValue(minChance, maxChance) <= _criticalChance;
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

            _turnsCount++;
        }

        public override Fighter Clone()
        {
            return new Fighter2(MaxHealth, Damage);
        }

        protected internal override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Каждую {_doubleAttackCount} атаку бьёт дважды!");
        }

        protected override bool TryApplySpecialAction()
        {
            Console.Write(" пробует второй удар иии...");

            return _turnsCount % _doubleAttackCount == 0;
        }
    }

    class Fighter3 : Fighter
    {
        private int _currentRage = 0;
        private int _rageByDamage = 5;
        private int _rageToHeal = 15;

        public Fighter3(int health, int damage) : base(health, damage)
        {
            Name = "Мазохист";
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

        public override Fighter Clone()
        {
            return new Fighter3(MaxHealth, Damage);
        }

        protected internal override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Копит получая урон {_rageByDamage} ярости, накопив {_rageToHeal} - преобразует её в здоровье.");
        }

        protected override bool TryApplySpecialAction()
        {
            Console.Write(" пытается преобразовать ярость в здоровье и...");
            return _currentRage == _rageToHeal;
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

        public override Fighter Clone()
        {
            return new Fighter4(MaxHealth, Damage);
        }

        protected internal override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Есть {_mana} маны. Файерболл наносит {_fireballDamage} урона, а стоит {_manaPriceFireball} маны. Накопление маны {_manaRegeneration} за ход.");
        }

        protected override bool TryApplySpecialAction()
        {
            Console.Write(" Пробует запустить файерболл и...");

            return _mana >= _manaPriceFireball;
        }

        private void SpendMana()
        {
            Console.WriteLine("Да! У него получается.");
            _mana -= _manaPriceFireball;
        }

        private void AccumulateMana()
        {
            Console.WriteLine("Нет! Просто копит ману.");
            _mana += _manaRegeneration;
        }
    }

    class Fighter5 : Fighter
    {
        private int _dodgeChance = 25;

        public Fighter5(int health, int damage) : base(health, damage)
        {
            Name = "Уворотчик";
        }

        public override void Attack(IDamageable target)
        {
            base.Attack(target);

            target.TakeDamage(Damage);
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

        public override Fighter Clone()
        {
            return new Fighter5(MaxHealth, Damage);
        }

        protected internal override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Имеет {_dodgeChance} процентный шанс уворота от любой атаки/заклинания.");
        }

        protected override bool TryApplySpecialAction()
        {
            int maxChance = 101;

            return UserUtills.ReturnRandomValue(0, maxChance) >= _dodgeChance;
        }
    }

    static class UserUtills
    {
        private static Random s_random = new Random();

        public static void CreateEmptyLine()
        {
            Console.Write("\n");
        }

        public static string ReturnSpacebar()
        {
            return " ";
        }

        public static int ReturnRandomValue(int leftBoundInclusive, int rightBound)
        {
            return s_random.Next(leftBoundInclusive, rightBound);
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

        public static string ReturnSeparator()
        {
            return " | ";
        }
    }
}

