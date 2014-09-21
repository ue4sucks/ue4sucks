using System.Text.RegularExpressions;

namespace UE4SUCKS.ViewModels
{
	sealed class AuthorizeViewModel : ViewModel
	{
		#region IsEnable

		bool isEnable = false;
		public bool IsEnable
		{
			get { return isEnable; }
			set
			{
				isEnable = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region Input

		string input = "";
		public string Input
		{
			get { return input; }
			set
			{
				input = value;
				RaisePropertyChanged();

				IsEnable = Verify(input);
			}
		}

		#endregion

		bool Verify(string value)
		{
			var pattern = new Regex(@"^\d{7}$");
			return pattern.IsMatch(value);
		}
	}
}
