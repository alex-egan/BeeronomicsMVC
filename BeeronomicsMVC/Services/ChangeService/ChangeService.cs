namespace BeeronomicsMVC.Services.ChangeService
{
    public class ChangeService : IChangeService
    {
        //public event Action<List<Drinks>> OnChange;

        //public void SendDrinks(List<Drinks> drinks)
        //{
        //    OnChange?.Invoke(drinks);
        //}
        public event Action<string> OnMessage;

        public void ClearMessages()
        {
            OnMessage?.Invoke(null);
        }

        public void SendMessage(string message)
        {
            OnMessage?.Invoke(message);
        }
    }
}
