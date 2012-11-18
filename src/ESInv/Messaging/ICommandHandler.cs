using System;


namespace ESInv.Messaging
{
	public interface ICommandHandler<TCommand>
		where TCommand : ICommand
	{
		void Handle(
			TCommand command);
	}
}
