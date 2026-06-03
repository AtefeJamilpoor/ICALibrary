using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;
using DimainLayer.Repository;

namespace ServiceLayer
{
    public class OverdueCheckerService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IMemberShipCardRepository _memberShipCardRepository;
        public OverdueCheckerService(IReservationRepository reservationRepository,IMemberShipCardRepository memberShipCardRepository)
        {
                _reservationRepository = reservationRepository;
            _memberShipCardRepository = memberShipCardRepository;
        }
        public async Task CheckOverdueUsersAsync()
        {
            //پیدا کردن کاربرانی که دیرکرد دارند
            List<int> overDueUsers = await  _reservationRepository.FindOverdueUsers();
            if(overDueUsers ==null)
                return;
             await _reservationRepository.MakeExpiered(overDueUsers);

            //پیدا کردن کاربرانی که از موعد دریافت رزروشان گذشته
            List<int> overDueUsersReservation = await _reservationRepository.FindOverdueUsersReservations();
            if (overDueUsersReservation == null)
                return;
            await _reservationRepository.MakeExpiered(overDueUsersReservation);

            //پیدا کردن کاربرانی که کارت اعتباریشان را تمدید نکردند
            List<int> overDueUsersCard = await _memberShipCardRepository.FindUsersOverdueCards();
            if( overDueUsersCard == null)
                return ;
            await _reservationRepository.MakeExpiered(overDueUsersCard);
        }
    }
}
