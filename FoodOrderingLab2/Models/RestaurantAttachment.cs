using System.ComponentModel.DataAnnotations;

namespace FoodOrderingLab2.Models;

public class RestaurantAttachment
{
    public int RestaurantAttachmentId { get; set; }
    public int RestaurantId { get; set; }

    [Required, StringLength(260)]
    public string FileName { get; set; } = null!;

    [Required, StringLength(500)]
    public string FilePath { get; set; } = null!;

    [Required, StringLength(150)]
    public string ContentType { get; set; } = null!;

    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
}
