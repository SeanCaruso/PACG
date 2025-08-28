namespace PACG.Gameplay
{
    public class AfterActingProcessor : BaseProcessor
    {
        private readonly EncounterContext _context;

        public AfterActingProcessor(EncounterContext context, GameServices gameServices)
            : base(gameServices)
        {
            _context = context;
        }

        protected override void OnExecute()
        {
            // TODO
        }
    }
}
