﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Xamarin.Forms;
using XForms.Constants;
using XForms.HttpREST;
using XForms.Models;
using XForms.Models.Projet;
using XForms.views.Conge;

namespace XForms.ViewModels
{
    public class NouvelleDemandeViewModel : BindableObject
    {
        
        public List<Conge> ListConge { get; set; }
        public List<Projet> ListProjet { get; set; }
        public List<SituationProjet> ListSituation { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public Conge SelectedConge { get; set; }
        public Projet SelectedProjet { get; set; }
        public SituationProjet SelectedSituationProjet { get; set; }

        public NouvelleDemandeViewModel()
        {
            DateDebut = DateTime.Now;
            DateFin = DateTime.Now;

            ListConge = new List<Conge> {
                new Conge(){
                    Type="Annuel"
                }
                ,
                new Conge(){
                    Type="Mensuel"
                }
            };

            ListProjet = new List<Projet> {
                new Projet(){
                    Name="Ta7alil"
                }
                ,
                new Projet(){
                    Name="Khdamat"
                }
            ,
                new Projet(){
                    Name="Kool"
                }
            ,
                new Projet(){
                    Name="ElectroPlanet"
                }
            ,
                new Projet(){
                    Name="Audit"
                }
            };

            ListSituation = new List<SituationProjet>
            {
                new SituationProjet(){
                    Name="Livré partiellement"
                },
                new SituationProjet(){
                    Name="Livré totalement"
                }
            };


            //API Post

            //Conge item = new Conge()
            //{
            //    Type = "Conge annuel",
            //    Status = "Reporté",
            //    DateDebut = DateDebut,
            //    DateFin = DateFin
            //};

            //var client = new HttpClient();

            //string json = JsonConvert.SerializeObject(item);
            //StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            //var responseMessage = client.PostAsync(AppUrls.GesRequestsListConge, content);
            
        }
        private bool CandSendRequest = true;

        public ICommand SendRequest => new Command(async () =>
            {
                try
                {
                    CandSendRequest = false;

                    Conge item = new Conge()
                    {
                        Type =SelectedConge.Name,
                        DateDebut = DateDebut,
                        DateFin = DateFin
                    };

                    //RESTServiceResponse<Conge> response = new RESTServiceResponse<Conge>();
                    //response.data = item;

                    //var client = new HttpClient();

                    //string json = JsonConvert.SerializeObject(item);
                    //StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    //var responseMessage = client.PostAsync(AppUrls.GesRequestsListConge, content);

                    var result = await App.AppServices.PostConge(item);

                    App.Current.MainPage.Navigation.PushAsync(new DemandeConge());

                }
                catch(Exception ex)
                {

                }
                finally
                {
                    CandSendRequest = true;
                }
            },
            ()=> CandSendRequest

            );
        public ICommand NavigationBack => new Command(() =>
        {
            //App.Current.MainPage.Navigation.PushAsync(new DemandeConge());
            App.Current.MainPage.Navigation.PopAsync();


        },
    () => true


    );


    }
}
