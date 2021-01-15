
namespace Shop.UIForms.Infrastructure
{
    using ViewModels;
    public class InstaceLocator
    {
        public MainViewModel Main { get; set; }

        public InstaceLocator()
        {
            this.Main = new MainViewModel();
        }
    }
}
