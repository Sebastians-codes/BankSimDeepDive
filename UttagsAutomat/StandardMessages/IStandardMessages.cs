using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashRegister
{
    /*
        Interface för alla meddelanden som används i programmet
        alla dessa metododer måste implementeras för att en ny språk klass ska fungera.
    */
    public interface IStandardMessages
    {
        void WelcomeMessage();
        void ShowCurrentPayedNSpent(decimal spent, decimal payed, string suffix);
        void ShowMenu();
        void HowMuchSpent();
        void HowMuchPayed();
        void NoItemsInCart();
        void NotPayedEnough();
        void RunAgain();
        void PayedEven();
        void ShowChange(decimal change, string suffix);
        void ConverterWorking();
        void ErrorTryAgain();
    }
}
