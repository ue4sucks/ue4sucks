/*--------------------------------------------------------------------------
* Livet ver 1.0.5 より一部利用
*
* author: ugaya40 (http://ugaya40.net/)
* license: zlib/libpng
* https://github.com/ugaya40/Livet/blob/ver1.0.5/.NET4.0/Livet(.NET4.0)/NotificationObject.cs#L47-56
*--------------------------------------------------------------------------*/

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UE4SUCKS
{
	abstract class ViewModel : INotifyPropertyChanged
	{
		static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> eventArgs = new ConcurrentDictionary<string, PropertyChangedEventArgs>();

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName="")
		{
			var threadSafeHandler = Interlocked.CompareExchange(ref PropertyChanged, null, null);
			if (threadSafeHandler != null)
			{
				var e = eventArgs.GetOrAdd(propertyName, name => new PropertyChangedEventArgs(name));
				threadSafeHandler(this, e);
			}
		}
	}
}
