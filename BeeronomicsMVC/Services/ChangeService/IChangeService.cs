namespace BeeronomicsMVC.Services.ChangeService
{
    public interface IChangeService
    {
        //event Action<List<Drinks>> OnChange;
        //void SendDrinks(List<Drinks> drinks);

        event Action<string> OnMessage;
        void SendMessage(string message);
        void ClearMessages();
    }
}
