using System;
using System.Collections.Generic;
using System.Globalization;
using Project_IT.Models;

namespace Project_IT.Services
{
    public class EmailTemplateBuilder
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Translations =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["mk"] = new Dictionary<string, string>
                {
                    ["lang_title"] = "Потврда за примено барање",
                    ["lang_header"] = "Барањето за резервација е примено!",
                    ["lang_status_banner"] = "Вашето барање е успешно испратено. Ќе ве контактираме наскоро за потврда.",
                    ["lang_greeting_intro"] = "Здраво",
                    ["lang_greeting_body"] = "Ви благодариме за интерес за престој во Автокамп Отешево! Вашето барање за резервација е успешно евидентирано. Во продолжение е преглед на податоците кои ги испративте:",
                    ["lang_summary_title"] = "Преглед на резервацијата",
                    ["lang_accommodation_label"] = "Сместување",
                    ["lang_guests_label"] = "Број на лица",
                    ["lang_checkin_label"] = "Дата на пристигнување",
                    ["lang_checkout_label"] = "Дата на заминување",
                    ["lang_phone_label"] = "Телефон за контакт",
                    ["lang_next_steps_title"] = "Што е следно?",
                    ["lang_next_steps_body"] = "Нашиот тим ќе ги провери расположливите капацитети за избраните дати и ќе ве контактира преку е-маил или телефон за конечно потврдување на вашата резервација.",
                    ["lang_footer_notice"] = "Доколку имате дополнителни прашања или сакате да направите промена, слободно одговорете на овој е-маил."
                },
                ["en"] = new Dictionary<string, string>
                {
                    ["lang_title"] = "Booking Request Received",
                    ["lang_header"] = "Reservation request received!",
                    ["lang_status_banner"] = "Your request has been successfully sent. We will contact you shortly for confirmation.",
                    ["lang_greeting_intro"] = "Hello",
                    ["lang_greeting_body"] = "Thank you for your interest in staying at Auto Camp Oteshevo! Your reservation request has been registered. Below is a summary of the details you submitted:",
                    ["lang_summary_title"] = "Reservation Summary",
                    ["lang_accommodation_label"] = "Accommodation",
                    ["lang_guests_label"] = "Number of guests",
                    ["lang_checkin_label"] = "Check-in Date",
                    ["lang_checkout_label"] = "Check-out Date",
                    ["lang_phone_label"] = "Contact phone",
                    ["lang_next_steps_title"] = "What's next?",
                    ["lang_next_steps_body"] = "Our team will check available capacity for your selected dates and contact you via email or phone to confirm your reservation.",
                    ["lang_footer_notice"] = "If you have any questions or wish to make changes, please reply directly to this email."
                },
                ["sq"] = new Dictionary<string, string>
                {
                    ["lang_title"] = "Kërkesa për rezervim u pranua",
                    ["lang_header"] = "Kërkesa për rezervim u pranua!",
                    ["lang_status_banner"] = "Kërkesa juaj u dërgua me sukses. Do t'ju kontaktojmë së shpejti për konfirmim.",
                    ["lang_greeting_intro"] = "Përshëndetje",
                    ["lang_greeting_body"] = "Faleminderit për interesimin tuaj për të qëndruar në Auto Camp Oteshevo! Kërkesa juaj për rezervim është regjistruar me sukses. Më poshtë është një përmbledhje e të dhënave që keni dërguar:",
                    ["lang_summary_title"] = "Përmbledhja e rezervimit",
                    ["lang_accommodation_label"] = "Akomodimi",
                    ["lang_guests_label"] = "Numri i personave",
                    ["lang_checkin_label"] = "Data e mbërritjes",
                    ["lang_checkout_label"] = "Data e largimit",
                    ["lang_phone_label"] = "Telefoni i kontaktit",
                    ["lang_next_steps_title"] = "Çfarë vijon më pas?",
                    ["lang_next_steps_body"] = "Ekipi ynë do të kontrollojë kapacitetet e lira për datat e zgjedhura dhe do t'ju kontaktojë përmes email-it ose telefonit për konfirmim.",
                    ["lang_footer_notice"] = "Nëse keni pyetje shtesë ose dëshironi të bëni ndryshime, mos hezitoni t'i përgjigjeni këtij email-i."
                },
                ["sr"] = new Dictionary<string, string>
                {
                    ["lang_title"] = "Potvrda o primljenom zahtevu",
                    ["lang_header"] = "Zahtev za rezervaciju je primljen!",
                    ["lang_status_banner"] = "Vaš zahtev je uspešno poslat. Kontaktiraćemo vas uskoro radi potvrde.",
                    ["lang_greeting_intro"] = "Zdravo",
                    ["lang_greeting_body"] = "Hvala vam na interesovanju za boravak u Auto Kampu Oteševo! Vaš zahtev za rezervaciju je uspešno zabeležen. U nastavku je pregled podataka koje ste poslali:",
                    ["lang_summary_title"] = "Pregled rezervacije",
                    ["lang_accommodation_label"] = "Smeštaj",
                    ["lang_guests_label"] = "Broj osoba",
                    ["lang_checkin_label"] = "Datum dolaska",
                    ["lang_checkout_label"] = "Datum odlaska",
                    ["lang_phone_label"] = "Kontakt telefon",
                    ["lang_next_steps_title"] = "Šta je sledeće?",
                    ["lang_next_steps_body"] = "Naš tim će proveriti raspoložive kapacitete za izabrane datume i kontaktiraće vas putem e-maila ili telefona radi konačne potvrde rezervacije.",
                    ["lang_footer_notice"] = "Ukoliko imate dodatna pitanja ili želite da napravite izmenu, slobodno odgovorite na ovaj e-mail."
                },
                ["de"] = new Dictionary<string, string>
                {
                    ["lang_title"] = "Buchungsanfrage erhalten",
                    ["lang_header"] = "Reservierungsanfrage eingegangen!",
                    ["lang_status_banner"] = "Ihre Anfrage wurde erfolgreich gesendet. Wir werden Sie kürze zur Bestätigung kontaktieren.",
                    ["lang_greeting_intro"] = "Hallo",
                    ["lang_greeting_body"] = "Vielen Dank für Ihr Interesse an einem Aufenthalt im Auto Camp Oteshevo! Ihre Reservierungsanfrage wurde erfolgreich erfasst. Nachfolgend finden Sie eine Übersicht Ihrer Angaben:",
                    ["lang_summary_title"] = "Zusammenfassung der Reservierung",
                    ["lang_accommodation_label"] = "Unterkunft",
                    ["lang_guests_label"] = "Anzahl der Gäste",
                    ["lang_checkin_label"] = "Anreisedatum",
                    ["lang_checkout_label"] = "Abreisedatum",
                    ["lang_phone_label"] = "Kontakttelefon",
                    ["lang_next_steps_title"] = "Wie geht es weiter?",
                    ["lang_next_steps_body"] = "Unser Team prüft die Verfügbarkeit für die gewählten Daten und wird Sie per E-Mail oder Telefon kontaktieren, um Ihre Buchung zu bestätigen.",
                    ["lang_footer_notice"] = "Wenn Sie weitere Fragen haben oder Änderungen vornehmen möchten, antworten Sie einfach auf diese E-Mail."
                },
                ["pl"] = new Dictionary<string, string>
                {
                    ["lang_title"] = "Otrzymano zapytanie o rezerwację",
                    ["lang_header"] = "Wniosek o rezerwację został przyjęty!",
                    ["lang_status_banner"] = "Twoje zapytanie zostało pomyślnie wysłane. Skontaktujemy się z Tobą wkrótce w celu potwierdzenia.",
                    ["lang_greeting_intro"] = "Cześć",
                    ["lang_greeting_body"] = "Dziękujemy za zainteresowanie pobytem w Auto Camp Oteshevo! Twoje zapytanie o rezerwację zostało zarejestrowane. Poniżej znajduje się podsumowanie przesłanych danych:",
                    ["lang_summary_title"] = "Podsumowanie rezerwacji",
                    ["lang_accommodation_label"] = "Zakwaterowanie",
                    ["lang_guests_label"] = "Liczba gości",
                    ["lang_checkin_label"] = "Data przyjazdu",
                    ["lang_checkout_label"] = "Data wyjazdu",
                    ["lang_phone_label"] = "Telefon kontaktowy",
                    ["lang_next_steps_title"] = "Co dalej?",
                    ["lang_next_steps_body"] = "Nasz zespół sprawdzi dostępność miejsc w wybranych terminach i skontaktuje się z Tobą e-mailem lub telefonicznie w celu ostatecznego potwierdzenia rezerwacji.",
                    ["lang_footer_notice"] = "Jeśli masz dodatkowe pytania lub chcesz dokonać zmian, odpowiedz bezpośrednio na ten e-mail."
                },
                ["it"] = new Dictionary<string, string>
                {
                    ["lang_title"] = "Richiesta di prenotazione ricevuta",
                    ["lang_header"] = "Richiesta di prenotazione ricevuta!",
                    ["lang_status_banner"] = "La tua richiesta è stata inviata con successo. Ti contatteremo a breve per la conferma.",
                    ["lang_greeting_intro"] = "Ciao",
                    ["lang_greeting_body"] = "Grazie per il tuo interesse a soggiornare presso l'Auto Camp Oteshevo! La tua richiesta di prenotazione è stata registrata. Di seguito un riepilogo dei dati inviati:",
                    ["lang_summary_title"] = "Riepilogo della prenotazione",
                    ["lang_accommodation_label"] = "Alloggio",
                    ["lang_guests_label"] = "Numero di ospiti",
                    ["lang_checkin_label"] = "Data di arrivo",
                    ["lang_checkout_label"] = "Data di partenza",
                    ["lang_phone_label"] = "Telefono di contatto",
                    ["lang_next_steps_title"] = "Cosa succede ora?",
                    ["lang_next_steps_body"] = "Il nostro team verificherà la disponibilità per le date selezionate e ti contatterà via e-mail o telefono per confermare la prenotazione.",
                    ["lang_footer_notice"] = "Se hai ulteriori domande o desideri apportare modifiche, rispondi pure a questa e-mail."
                },
                ["el"] = new Dictionary<string, string>
                {
                    ["lang_title"] = "Επιβεβαίωση λήψης αιτήματος",
                    ["lang_header"] = "Το αίτημα κράτησης ελήφθη!",
                    ["lang_status_banner"] = "Το αίτημά σας στάλθηκε με επιτυχία. Θα επικοινωνήσουμε μαζί σας σύντομα για επιβεβαίωση.",
                    ["lang_greeting_intro"] = "Γεια σας",
                    ["lang_greeting_body"] = "Σας ευχαριστούμε για το ενδιαφέρον σας να μείνετε στο Auto Camp Oteshevo! Το αίτημα κράτησής σας έχει καταγραφεί. Παρακάτω ακολουθεί μια σύνοψη των στοιχείων που υποβάλατε:",
                    ["lang_summary_title"] = "Σύνοψη κράτησης",
                    ["lang_accommodation_label"] = "Διαμονή",
                    ["lang_guests_label"] = "Αριθμός ατόμων",
                    ["lang_checkin_label"] = "Ημερομηνία άφιξης",
                    ["lang_checkout_label"] = "Ημερομηνία αναχώρησης",
                    ["lang_phone_label"] = "Τηλέφωνο επικοινωνίας",
                    ["lang_next_steps_title"] = "Τι ακολουθεί;",
                    ["lang_next_steps_body"] = "Η ομάδα μας θα ελέγξει τη διαθεσιμότητα για τις επιλεγμένες ημερομηνίες και θα επικοινωνήσει μαζί σας μέσω email ή τηλεφώνου για την τελική επιβεβαίωση της κράτησής σας.",
                    ["lang_footer_notice"] = "Εάν έχετε επιπλέον ερωτήσεις ή θέλετε να κάνετε κάποια αλλαγή, μη διστάσετε να απαντήσετε σε αυτό το email."
                },
                ["es"] = new Dictionary<string, string>
                {
                    ["lang_title"] = "Solicitud de reserva recibida",
                    ["lang_header"] = "¡Solicitud de reserva recibida!",
                    ["lang_status_banner"] = "Su solicitud ha sido enviada con éxito. Nos pondremos en contacto pronto para la confirmación.",
                    ["lang_greeting_intro"] = "Hola",
                    ["lang_greeting_body"] = "¡Gracias por su interés en alojarse en Auto Camp Oteshevo! Su solicitud de reserva ha sido registrada. A continuación se muestra un resumen de los datos enviados:",
                    ["lang_summary_title"] = "Resumen de la reserva",
                    ["lang_accommodation_label"] = "Alojamiento",
                    ["lang_guests_label"] = "Número de huéspedes",
                    ["lang_checkin_label"] = "Fecha de entrada",
                    ["lang_checkout_label"] = "Fecha de salida",
                    ["lang_phone_label"] = "Teléfono de contacto",
                    ["lang_next_steps_title"] = "¿Qué sigue?",
                    ["lang_next_steps_body"] = "Nuestro equipo verificará la disponibilidad para las fechas seleccionadas y se pondrá en contacto con usted por correo electrónico o teléfono para confirmar la reserva.",
                    ["lang_footer_notice"] = "Si tiene alguna pregunta adicional o desea realizar cambios, no dude en responder a este correo electrónico."
                }
            };

        public static string GetSubjectForCulture(string culture)
        {
            var dictionary = ResolveDictionary(culture);
            if (dictionary.TryGetValue("lang_title", out var title))
            {
                return $"{title} - Auto Camp Oteshevo";
            }
            return "Потврда за примено барање - Автокамп Отешево";
        }

        private static Dictionary<string, string> ResolveDictionary(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return Translations["mk"];
            }

            var cleanCulture = culture.Trim().ToLowerInvariant();
            if (cleanCulture.Contains("-"))
            {
                cleanCulture = cleanCulture.Split('-')[0];
            }
            if (cleanCulture.Contains("_"))
            {
                cleanCulture = cleanCulture.Split('_')[0];
            }

            if (Translations.TryGetValue(cleanCulture, out var dict))
            {
                return dict;
            }

            return Translations["mk"];
        }

        public static string BuildAdminNotificationEmail(ReservationSubmissionModel model)
        {
            const string template = @"<!DOCTYPE html>
<html lang=""mk"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Нова Резервација - Автокамп Отешево</title>
</head>
<body style=""margin: 0; padding: 20px 0; background-color: #f1f5f9; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;"">

  <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"">
    <tr>
      <td align=""center"">
        <!-- Main Email Container -->
        <table role=""presentation"" width=""100%"" style=""max-width: 600px; background-color: #ffffff; border-radius: 12px; border: 1px solid #e2e8f0; border-spacing: 0; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05);"">

          <!-- Header Bar -->
          <tr>
            <td style=""background-color: #064e3b; padding: 24px 32px; text-align: left;"">
              <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                <tr>
                  <td>
                    <span style=""display: inline-block; background-color: rgba(255, 255, 255, 0.15); color: #6ee7b7; font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: 1.2px; padding: 4px 10px; border-radius: 9999px; margin-bottom: 8px;"">
                      Известување
                    </span>
                    <h1 style=""color: #ffffff; margin: 0; font-size: 22px; font-weight: 700; font-family: 'Georgia', serif;"">
                      Нова резервација
                    </h1>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Body Content -->
          <tr>
            <td style=""padding: 32px;"">

              <!-- Guest Information Section -->
              <div style=""margin-bottom: 24px;"">
                <h2 style=""font-size: 12px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: #64748b; margin: 0 0 12px 0;"">
                  Податоци за гостинот
                </h2>
                <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 16px;"">
                  <tr>
                    <td style=""padding-bottom: 8px; font-size: 15px; font-weight: 700; color: #0f172a;"">
                      {{customer_name}}
                    </td>
                  </tr>
                  <tr>
                    <td style=""padding-bottom: 4px; font-size: 14px; color: #334155;"">
                      <strong>Email:</strong> <a href=""mailto:{{customer_email}}"" style=""color: #059669; text-decoration: none; font-weight: 600;"">{{customer_email}}</a>
                    </td>
                  </tr>
                  <tr>
                    <td style=""font-size: 14px; color: #334155;"">
                      <strong>Телефон:</strong> <a href=""tel:{{phone}}"" style=""color: #059669; text-decoration: none; font-weight: 600;"">{{phone}}</a>
                    </td>
                  </tr>
                </table>
              </div>

              <!-- Reservation Details Section -->
              <div style=""margin-bottom: 24px;"">
                <h2 style=""font-size: 12px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: #64748b; margin: 0 0 12px 0;"">
                  Детали за престој
                </h2>
                <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""border-collapse: collapse; font-size: 14px;"">
                  <tr style=""border-bottom: 1px solid #f1f5f9;"">
                    <td style=""padding: 10px 0; color: #64748b; width: 45%;"">Сместување:</td>
                    <td style=""padding: 10px 0; color: #0f172a; font-weight: 600;"">{{smestuvanje}}</td>
                  </tr>
                  <tr style=""border-bottom: 1px solid #f1f5f9;"">
                    <td style=""padding: 10px 0; color: #64748b;"">Број на лица:</td>
                    <td style=""padding: 10px 0; color: #0f172a; font-weight: 600;"">{{guests}}</td>
                  </tr>
                  <tr style=""border-bottom: 1px solid #f1f5f9;"">
                    <td style=""padding: 10px 0; color: #64748b;"">Дата на пристигнување:</td>
                    <td style=""padding: 10px 0; color: #059669; font-weight: 700;"">{{check_in}}</td>
                  </tr>
                  <tr>
                    <td style=""padding: 10px 0; color: #64748b;"">Дата на заминување:</td>
                    <td style=""padding: 10px 0; color: #dc2626; font-weight: 700;"">{{check_out}}</td>
                  </tr>
                </table>
              </div>

              <!-- Message / Special Requests -->
              <div style=""margin-bottom: 28px;"">
                <h2 style=""font-size: 12px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: #64748b; margin: 0 0 8px 0;"">
                  Дополнителна порака / Забелешка
                </h2>
                <div style=""background-color: #ffffff; border: 1px dashed #cbd5e1; border-radius: 8px; padding: 14px 16px; font-size: 14px; color: #334155; line-height: 1.5; font-style: italic;"">
                  {{message}}
                </div>
              </div>

            </td>
          </tr>

          <!-- Technical Diagnostic Metadata -->
          <tr>
            <td style=""background-color: #f8fafc; border-top: 1px solid #e2e8f0; padding: 20px 32px; font-size: 11px; color: #94a3b8; line-height: 1.6;"">
              OS: {{user_os}} &bull; Platform: {{user_platform}} &bull; Browser: {{user_browser}}<br />
              Referrer: <a href=""{{user_referrer}}"" style=""color: #94a3b8;"">{{user_referrer}}</a>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>

</body>
</html>";

            return ReplaceSharedPlaceholders(template, model);
        }

        public static string BuildClientConfirmationEmail(ReservationSubmissionModel model, string culture)
        {
            var dict = ResolveDictionary(culture);

            const string baseTemplate = @"<!DOCTYPE html>
<html>
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>{{lang_title}} - Auto Camp Oteshevo</title>
</head>
<body style=""margin: 0; padding: 20px 0; background-color: #f1f5f9; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;"">

  <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"">
    <tr>
      <td align=""center"">
        <!-- Main Email Container -->
        <table role=""presentation"" width=""100%"" style=""max-width: 600px; background-color: #ffffff; border-radius: 12px; border: 1px solid #e2e8f0; border-spacing: 0; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05);"">

          <!-- Header Bar -->
          <tr>
            <td style=""background-color: #064e3b; padding: 28px 32px; text-align: center;"">
              <span style=""display: inline-block; background-color: rgba(255, 255, 255, 0.15); color: #6ee7b7; font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: 1.2px; padding: 4px 12px; border-radius: 9999px; margin-bottom: 10px;"">
                Auto Camp Oteshevo
              </span>
              <h1 style=""color: #ffffff; margin: 0; font-size: 22px; font-weight: 700; font-family: 'Georgia', serif;"">
                {{lang_header}}
              </h1>
            </td>
          </tr>

          <!-- Confirmation Status Banner -->
          <tr>
            <td style=""background-color: #ecfdf5; border-bottom: 1px solid #a7f3d0; padding: 14px 32px; font-size: 13px; color: #065f46; text-align: center;"">
              {{lang_status_banner}}
            </td>
          </tr>

          <!-- Body Content -->
          <tr>
            <td style=""padding: 32px;"">

              <!-- Greeting -->
              <div style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 24px;"">
                {{lang_greeting_intro}} <strong>{{customer_name}}</strong>,<br /><br />
                {{lang_greeting_body}}
              </div>

              <!-- Reservation Summary Table -->
              <div style=""margin-bottom: 28px;"">
                <h2 style=""font-size: 12px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: #64748b; margin: 0 0 12px 0;"">
                  {{lang_summary_title}}
                </h2>
                <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 16px; border-collapse: collapse; font-size: 14px;"">
                  <tr style=""border-bottom: 1px solid #e2e8f0;"">
                    <td style=""padding: 10px 0; color: #64748b; width: 45%;"">{{lang_accommodation_label}}:</td>
                    <td style=""padding: 10px 0; color: #0f172a; font-weight: 600;"">{{smestuvanje}}</td>
                  </tr>
                  <tr style=""border-bottom: 1px solid #e2e8f0;"">
                    <td style=""padding: 10px 0; color: #64748b;"">{{lang_guests_label}}:</td>
                    <td style=""padding: 10px 0; color: #0f172a; font-weight: 600;"">{{guests}}</td>
                  </tr>
                  <tr style=""border-bottom: 1px solid #e2e8f0;"">
                    <td style=""padding: 10px 0; color: #64748b;"">{{lang_checkin_label}}:</td>
                    <td style=""padding: 10px 0; color: #059669; font-weight: 700;"">{{check_in}}</td>
                  </tr>
                  <tr style=""border-bottom: 1px solid #e2e8f0;"">
                    <td style=""padding: 10px 0; color: #64748b;"">{{lang_checkout_label}}:</td>
                    <td style=""padding: 10px 0; color: #004731; font-weight: 700;"">{{check_out}}</td>
                  </tr>
                  <tr>
                    <td style=""padding: 10px 0; color: #64748b;"">{{lang_phone_label}}:</td>
                    <td style=""padding: 10px 0; color: #0f172a; font-weight: 600;"">{{phone}}</td>
                  </tr>
                </table>
              </div>

              <!-- Next Steps Info Box -->
              <div style=""background-color: #f1f5f9; border-left: 4px solid #059669; border-radius: 4px; padding: 14px 16px; font-size: 13px; color: #475569; line-height: 1.5; margin-bottom: 28px;"">
                <strong style=""color: #0f172a;"">{{lang_next_steps_title}}</strong><br />
                {{lang_next_steps_body}}
              </div>

              <!-- Contact Info Notice -->
              <div style=""font-size: 13px; color: #64748b; line-height: 1.5; text-align: center;"">
                {{lang_footer_notice}}
              </div>

            </td>
          </tr>

          <!-- Footer Bar -->
          <tr>
            <td style=""background-color: #f8fafc; border-top: 1px solid #e2e8f0; padding: 20px 32px; font-size: 12px; color: #94a3b8; text-align: center; line-height: 1.5;"">
              <strong style=""color: #64748b;"">Auto Camp Oteshevo</strong><br />
              Prespa, Republic of North Macedonia
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>

</body>
</html>";

            var html = baseTemplate;
            foreach (var kvp in dict)
            {
                html = html.Replace("{{" + kvp.Key + "}}", kvp.Value);
            }

            return ReplaceSharedPlaceholders(html, model);
        }

        private static string ReplaceSharedPlaceholders(string template, ReservationSubmissionModel model)
        {
            if (model == null) return template;

            return template
                .Replace("{{customer_name}}", model.FullName ?? string.Empty)
                .Replace("{{customer_email}}", model.Email ?? string.Empty)
                .Replace("{{phone}}", model.Phone ?? string.Empty)
                .Replace("{{smestuvanje}}", model.AccommodationType ?? string.Empty)
                .Replace("{{guests}}", model.Guests ?? string.Empty)
                .Replace("{{check_in}}", model.CheckInDate ?? string.Empty)
                .Replace("{{check_out}}", model.CheckOutDate ?? string.Empty)
                .Replace("{{message}}", model.Info ?? string.Empty)
                .Replace("{{user_os}}", model.UserOs ?? string.Empty)
                .Replace("{{user_platform}}", model.UserPlatform ?? string.Empty)
                .Replace("{{user_browser}}", model.UserBrowser ?? string.Empty)
                .Replace("{{user_referrer}}", model.UserReferrer ?? string.Empty);
        }
    }
}
