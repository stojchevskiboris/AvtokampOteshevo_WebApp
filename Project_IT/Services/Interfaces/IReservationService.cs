using System.Collections.Generic;
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
        bool ProcessReservationSubmission(ReservationSubmissionModel model, string userAgent, string userHostAddress, out string errorMessage, out Dictionary<string, IEnumerable<string>> validationErrors);
    }
}
