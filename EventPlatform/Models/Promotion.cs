using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EventPlatform.Models
{
    public class Promotion
    {
        [Key] public int Id { get; set; }
        public OrderState State { get; set; }
        public DateTime Date { get; set; }
        public byte[] Image { get; set; }
        public string Annotation { get; set; }

        public int User_id { get; set; }
        public int Event_id { get; set; }

        public static List<Promotion> SelectList(int option)
        {
            using (var db = new Models.ModelContext())
            {
                if (0 <= option && option < 3)
                {
                    return db.Promotions.Where(e => e.State==(OrderState)option).ToList();
                }
                else
                {
                    return db.Promotions.ToList();
                }
            }
        }

        public static Tuple<Promotion, string,string> Select(int promotionId)
        {
            using (var db = new Models.ModelContext())
            {
                string eventName = "Nėra";
                string organizerName = "Nėra";
                Promotion promotion = db.Promotions.FirstOrDefault(p => p.Id.Equals(promotionId));
                if(promotion != null)
                {
                organizerName = db.Users.FirstOrDefault(u => u.Id.Equals(promotion.User_id)).Username;
                eventName = db.Events.FirstOrDefault(e => e.Id.Equals(promotion.Event_id)).Name;
                }

                return new Tuple<Promotion, string, string>(promotion,organizerName, eventName);
            }
        }

        public static string Update(int promotionId, int state)
        {
            using (var db = new Models.ModelContext())
            {
                if (0 <= state && state < 3)
                {
                    var currPromotion = db.Promotions.FirstOrDefault(p => p.Id.Equals(promotionId));
                    if (currPromotion != null)
                    {
                        if (currPromotion.State==(OrderState)state)
                            return "Negalima pekeisti užsakymo būsenos į tokią pačia";
                        else
                        {
                            currPromotion.State = (OrderState)state;
                            db.Update(currPromotion);
                            db.SaveChanges();
                            return "Užsakymo būsena sėkmingai atnaujinta";
                        }
                    }
                    else
                        return "Šis užsakymas nerastas";
                }
                else
                    return "Neteisinga užsakymo būsena";
            }
        }
        public static string getStateString(OrderState state)
        {
            if (OrderState.approved == state)
            {
                return "Patvirtintas";
            } else if (OrderState.rejected== state)
            {
                return "Atšauktas";
            } else if (OrderState.waitingApproval == state)
            {
                return "Laukiantis patvirtinimo";
            }
            else
            {
                return "Nenumatyta būsena.";
            }
        }
    }

    public enum OrderState
    {
        approved,
        waitingApproval,
        rejected
    }
}
