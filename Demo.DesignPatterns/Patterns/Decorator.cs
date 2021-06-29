using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class Decorator
    {
        public void ConsumeThePattern()
        {
            Game game1 = new Football();
            // so now if we need to injest the uniform for example
            game1 = new UniForm(game1);
            game1.Play();
        }
    }

    public abstract class Game
    {
        public abstract void Play();
    }

    public class Football : Game
    {
        public override void Play()
        {
            Console.WriteLine("Playing Football");
        }
    }

    public class Tennis : Game
    {
        public override void Play()
        {
            Console.WriteLine("Playing Tennis");
        }
    }


    public abstract class GameRequirmentsDecorator : Game
    {
        public Game game;
        public GameRequirmentsDecorator(Game _game)
        {
            this.game = _game;
        }
        public override void Play()
        {
            this.game.Play();
            // do whatever additional functionality
        }
    }

    public class UniForm : GameRequirmentsDecorator
    {
        public UniForm(Game _game) : base(_game)
        {
        }

        public void Wear()
        {
            Console.WriteLine("Wearing UniForm");
        }
    }

    public class PlayGround : GameRequirmentsDecorator
    {
        public PlayGround(Game _game) : base(_game)
        {
        }
        public void Book()
        {
            Console.WriteLine("Booking Playground");
        }
    }
}
