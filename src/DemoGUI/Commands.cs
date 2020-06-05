using System.Windows.Input;

namespace Steganography
{
    public static class Commands
    {
        public static readonly RoutedUICommand Encode = new RoutedUICommand(
            "Encode",
            "Encode",
            typeof(Commands)
        );

        public static readonly RoutedUICommand Decode = new RoutedUICommand(
            "Decode",
            "Decode",
            typeof(Commands)
        );

        public static readonly RoutedUICommand SelectMedium = new RoutedUICommand(
            "SelectMedium",
            "SelectMedium",
            typeof(Commands)
        );

        public static readonly RoutedUICommand SelectSecret = new RoutedUICommand(
            "SelectSecret",
            "SelectSecret",
            typeof(Commands)
        );

        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Exit",
            "Exit",
            typeof(Commands)
        );
    }
}
