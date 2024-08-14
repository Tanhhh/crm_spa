using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

public class CrmTemplateViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Hãy chọn loại tin nhắn!")]
    [Display(Name = "Loại tin nhắn")]
    public int TypeTemplate { get; set; }

    //[Required(ErrorMessage = "Hãy nhập nội dung tin nhắn!")]
    [Display(Name = "Nội dung tin nhắn")]
    public string ContentRule { get; set; }

   // [Required(ErrorMessage = "Hãy nhập tiêu đề Email!")]
    [Display(Name = "Tiêu đê Email")]
    public string TileEmail { get; set; }

    //[Required(ErrorMessage = "Hãy nhập nội dung Email!")]
    [Display(Name = "Nội dung Email")]
    public string ContentEmail { get; set; }

    [Display(Name = "Ghi chú")]
    public string Note { get; set; }

    [Display(Name = "IsDeleted")]
    public bool IsDeleted { get; set; }

    [Display(Name = "CreatedDate")]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "CreatedUserId")]
    public int CreatedUserId { get; set; }

    [Display(Name = "ModifiedDate")]
    public DateTime? ModifiedDate { get; set; }

    [Display(Name = "ModifiedUserId")]
    public int ModifiedUserId { get; set; }

    //[Required]
    [Display(Name = "AssignedUserId")]
    public int AssignedUserId { get; set; }
    [Display(Name = "TypeLead")]
    public bool TypeLead { get; set; }
    [Display(Name = "ZNSId")]
    public string ZNSId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ((TypeTemplate == 1 || TypeTemplate == 2)   && ContentRule == null)
        {
            yield return new ValidationResult("Hãy nhập nội dung tin nhắn!");
        }
        if (TypeTemplate == 3 && (ContentEmail == null || TileEmail == null))
        {
            yield return new ValidationResult("Hãy nhập tiêu đề và nội dung Email!");
        }
        if (TypeTemplate == 1 && ContentRule.Length >= 260)
        {
            yield return new ValidationResult("Nội dung của tin nhắn SMS không được nhiều hơn 260 ký tự!");
        }
        if (TypeTemplate == 2 && ZNSId == null)
        {
            yield return new ValidationResult("Hãy nhập ZNS Id!");
        }

    }
}