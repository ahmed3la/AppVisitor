using System;
using System.Collections.Generic;
using System.Linq;

namespace AppVisitor
{
    //https://lostechies.com/jimmybogard/2007/10/24/entity-validation-with-visitors-and-extension-methods/
    class Program
    {

        static void Main(string[] args)
        {


            Order order = new Order();
            //OrderPersistenceValidator validator = new OrderPersistenceValidator();

            //IEnumerable<string> brokenRules;
            //bool isValid = order.Validate(validator, out brokenRules);

            IEnumerable<string> brokenRules;
            bool isValid = order.ValidatePersistence(out brokenRules);
            if (!isValid)
            { 
                throw new Exception("The Order is not valid!"); 
            }
        }
    }
    
    public static class ValidatorExtension
    {
        public static bool ValidatePersistence(this Order entity, out IEnumerable<string> brokenRules)
        {
            IValidator<Order> validator = new OrderPersistenceValidator();

            return entity.Validate(validator, out brokenRules);
        }
    }

    public class OrderPersistenceValidator : IValidator<Order>
    {
        public bool IsValid(Order entity)
        {
            return !BrokenRules(entity).Any();
        }

        public IEnumerable<string> BrokenRules(Order entity)
        {
            if (entity.Id < 0)
                yield return "Id cannot be less than 0.";

            if (string.IsNullOrEmpty(entity.Customer))
                yield return "Must include a customer.";

            yield break;
        }

    }

    public interface IValidatable<T>
    {
        bool Validate(IValidator<T> validator, out IEnumerable<string> brokenRules);
    }
    public class Order : IValidatable<Order>
    {
        public int Id { get; set; }
        public string Customer { get; set; }

        public bool Validate(IValidator<Order> validator, out IEnumerable<string> brokenRules)
        {
            brokenRules = validator.BrokenRules(this);
            return validator.IsValid(this);
        } 
    }

    public interface IValidator<T>
    {
        bool IsValid(T entity);
        IEnumerable<string> BrokenRules(T entity);
    }








}
