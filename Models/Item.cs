namespace GoblineerNextApi.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int InternalId { get; set; }
        public int? Context { get; set; }
        public int[]? Modifiers { get; set; }
        public int[]? Bonuses { get; set; }
        public int? PetBreedId { get; set; }
        public int? PetLevel { get; set; }
        public int? PetQualityId { get; set; }
        public int? PetSpeciesId { get; set; }
    }
}