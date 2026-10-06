using System.Collections.Generic;
using System.Threading.Tasks;
using Project_IT.Models;

namespace Project_IT.Services.Interfaces
{
    public interface IReservationService
    {
        IEnumerable<Reservation> GetAllReservations();
        Reservation GetReservationById(int id);
        void CreateReservation(Reservation reservation);
        void UpdateReservation(Reservation reservation);
        void DeleteReservation(int id);
        Task<ReservationSubmissionResult> ProcessReservationSubmissionAsync(ReservationSubmissionModel model, string userAgent, string userHostAddress, string culture = null);
    }
}
