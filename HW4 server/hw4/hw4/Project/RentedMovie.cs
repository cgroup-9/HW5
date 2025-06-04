namespace hw4.Project
{
    public class RentedMovie
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime RentStart { get; set; }
        public DateTime RentEnd { get; set; }
        public float TotalPrice { get; set; }
        public DateTime? DeletedAt { get; set; }

        public int Rent()
        {
            DBservices db = new DBservices();
            return db.RentMovie(this);
        }
        public int ForwardToAnotherUser(string toUserName)
        {
            DBservices db = new DBservices();
            return db.ForwardRentedMovie(this.UserId, this.MovieId, toUserName);
        }
    }

}
