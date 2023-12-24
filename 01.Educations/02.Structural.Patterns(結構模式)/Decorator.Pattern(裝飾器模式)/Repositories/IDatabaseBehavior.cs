namespace Decorator.Pattern_裝飾器模式_.Repositories;
internal interface IDatabaseBehavior
{
    void Create();
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class DatabaseBehavior : IDatabaseBehavior
{
    public void Create()
    {
        IDatabaseOperation operation = new DeleteOperation(
            new UpdateOperation(
                new ReadOperation(
                    new CreateOperation(
                        new BasicOperation()))));
        operation.Execute();

    }
    interface IDatabaseOperation
    {
        void Execute();
    }
    class BasicOperation : IDatabaseOperation
    {
        public void Execute()
        {
            //基本操作的實現
        }
    }
    abstract class DatabaseOperationDecorator(IDatabaseOperation databaseOperation) : IDatabaseOperation
    {
        public virtual void Execute()
        {
            databaseOperation.Execute();
        }
    }
    class CreateOperation(IDatabaseOperation databaseOperation) : DatabaseOperationDecorator(databaseOperation)
    {
        public override void Execute()
        {
            //執行新增操作
            base.Execute();
        }
    }
    class ReadOperation(IDatabaseOperation databaseOperation) : DatabaseOperationDecorator(databaseOperation)
    {
        public override void Execute()
        {
            //執行讀取操作
            base.Execute();
        }
    }
    class UpdateOperation(IDatabaseOperation databaseOperation) : DatabaseOperationDecorator(databaseOperation)
    {
        public override void Execute()
        {
            //執行更新操作
            base.Execute();
        }
    }
    class DeleteOperation(IDatabaseOperation databaseOperation) : DatabaseOperationDecorator(databaseOperation)
    {
        public override void Execute()
        {
            //執行刪除操作
            base.Execute();
        }
    }
}