﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using XForms.Models;
using XForms.views;

namespace XForms.ViewModels
{
    public class ComplaintAdministrationViewModel : BaseViewModel
    {
        public ObservableRangeCollection<ComplaintResponse> ComplaintProfils { get; set; }

        public ObservableRangeCollection<ComplaintResponse> ProfilsConfirmedComplaintList { get; set; }
        public ObservableRangeCollection<ComplaintResponse> ProfilsInProgressComplaintList { get; set; }

        public bool IsComplaintRequestInProgress { get; set; }
        public bool IsComplaintRequestConfirmed { get; set; }

        public ObservableRangeCollection<ComplaintResponse> ProfilsComplaintItemsList { get; set; }

        public ComplaintResponse SelectedComplaint { get; set; }

        public int numberOfRequestsAdmin { get; set; }
        public ComplaintAdministrationViewModel()
        {

        }
        public async override void OnAppearing()
        {
            base.OnAppearing();
            IsComplaintRequestInProgress = true;

            await GetAllComplaints();

            //ProfilsComplaintItemsList = new ObservableRangeCollection<ComplaintResponse>();
            //ProfilsComplaintItemsList = ProfilsInProgressComplaintList;
        }


        public async Task GetAllComplaints()
        {
            AppHelpers.LoadingShow();
            var result = await App.AppServices.GetAllComplaint();
            if (result?.succeeded == true)
            {
                ComplaintProfils = new ObservableRangeCollection<ComplaintResponse>(result.data.ToList());

                ProfilsConfirmedComplaintList = new ObservableRangeCollection<ComplaintResponse>(result.data.Where(x => (x.RefStatusClaimId == 2)).ToList());
                ProfilsInProgressComplaintList = new ObservableRangeCollection<ComplaintResponse>(result.data.Where(x => (x.RefStatusClaimId == 1)).ToList());

                ProfilsComplaintItemsList = HeadrActionList[0].IsSelected ? ProfilsInProgressComplaintList : ProfilsConfirmedComplaintList;
                numberOfRequestsAdmin = ProfilsComplaintItemsList.Count;
            }
            else
            {
                AppHelpers.Alert(result?.message);
            }
            AppHelpers.LoadingHide();
        }


        private bool CanSelectHeaderAction = true;
        public ICommand SelectHeaderActionCommand => new Command<REFItem>(async (model) =>
        {
            try
            {
                CanSelectHeaderAction = false;

                if (model == null) return;

                foreach (var item in HeadrActionList)
                {
                    item.IsSelected = (item.Id == model.Id);
                    OnPropertyChanged(nameof(item.IsSelected));
                }

                IsComplaintRequestInProgress = HeadrActionList[0].IsSelected;
                IsComplaintRequestConfirmed = !IsComplaintRequestInProgress;

                ProfilsComplaintItemsList = HeadrActionList[0].IsSelected ? ProfilsInProgressComplaintList : ProfilsConfirmedComplaintList;
                numberOfRequestsAdmin = ProfilsComplaintItemsList.Count;

            }
            catch (Exception ex)
            {

            }
            finally
            {
                CanSelectHeaderAction = true;
            }
        },

        (_) => CanSelectHeaderAction);

        private ProfilComplaintPopup profilComplaintPopup;
        private bool canprofilComplaintPopup = true;

        public ICommand OpenComplaintDetailsPopupCommand => new Command<ComplaintResponse>(async (model) =>
        {
            try
            {
                canprofilComplaintPopup = false;

                if (profilComplaintPopup == null)
                    profilComplaintPopup = new ProfilComplaintPopup() { BindingContext = this };

                SelectedComplaint = model;

                await PopupNavigation.Instance.PushSingleAsync(profilComplaintPopup);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                canprofilComplaintPopup = true;
            }


        }, (_) => canprofilComplaintPopup);

        private bool canConfirmDispalacement = true;
        public ICommand ConfirmeComplaintCommand => new Command(async () =>
        {
            try
            {
                canConfirmDispalacement = false;
                AppHelpers.LoadingShow();

                var postParams = new Models.ComplaintTraitement()
                {
                    Id = SelectedComplaint.Id,
                    RefStatusClaimId = 2
                };
                var result = await App.AppServices.PosteUpdateComplaint(postParams);

                await GetAllComplaints();

                await PopupNavigation.Instance.PopAllAsync();
                AppHelpers.LoadingHide();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                canConfirmDispalacement = true;
            }

        }, () => canConfirmDispalacement);

        private bool canRejectComplaint = true;
        public ICommand RejectComplaintCommand => new Command(async () =>
        {
            try
            {
                canRejectComplaint = false;
                AppHelpers.LoadingShow();

                var postParams = new Models.ComplaintTraitement()
                {
                    Id = SelectedComplaint.Id,
                    RefStatusClaimId = 3
                };
                var result = await App.AppServices.PosteUpdateComplaint(postParams);

                //ProfilsDispalacementItemsList.Remove()

                await GetAllComplaints();

                await PopupNavigation.Instance.PopAllAsync();
                AppHelpers.LoadingHide();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                canRejectComplaint = true;
            }

        }, () => canRejectComplaint);
    }
}
