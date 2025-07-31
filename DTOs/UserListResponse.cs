namespace FashionStoreAPI.DTOs
{
    public class UserListResponse
    {
        public int TotalNumberOfUsers { get; set; }

        public List<BasicUserResponse> Users { get; set; } = [];
    }
}
