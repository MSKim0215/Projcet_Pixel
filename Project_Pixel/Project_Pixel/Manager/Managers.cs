namespace Threading.Manager
{
    public class Managers : Core
    {
        private static Managers instance;

        private Managers() { }

        public static Managers Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Managers();
                }
                return instance;
            }
        }

        private readonly GameManager game = new GameManager();
        private readonly UIManager ui = new UIManager();
        private readonly DataManager data = new DataManager();

        public static GameManager Game => Instance.game;
        public static UIManager UI => Instance.ui;
        public static DataManager Data => Instance.data;

        public override void Start()
        {
            UI.Init();
            Data.Init();
            Game.Init();

            base.Start();
        }

        public override void Update()
        {
            //Game.OnUpdate();
            //Data.OnUpdate();
        }
    }
}
