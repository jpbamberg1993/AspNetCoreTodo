using System;
using AspNetCoreTodo.Interfaces;

namespace AspNetCoreTodo.Models
{
    public class Operation : IOperationTransient, IOperationScoped, IOperationSingleton, IOperationSingletonInstance
    {
        private Guid _guid;
        
        public Operation() : this(Guid.NewGuid()) {}

        public Operation(Guid guid)
        {
            _guid = guid;
        }

        public Guid OperationId => _guid;
    }
}