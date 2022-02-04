namespace PasswordManager.Database.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class IdentityEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public void Validate()
        {
            var validationContext = new ValidationContext(this);
            Validator.ValidateObject(
                this,
                validationContext,
                validateAllProperties: true);
            return;
        }

        public string TrappedValidate()
        {
            try
            {
                Validate();
                return string.Empty;
            }
            catch (ValidationException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return $"Validation Exception - {ex.Message}";
            }
        }
    }
}
