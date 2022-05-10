﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using XForms.Models;
using XForms.views;
using XForms.views.Project;

namespace XForms.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class ProjectViewModel :BaseViewModel
    {
        //public ObservableRangeCollection<Project> Projects { get; set; }
        public ObservableRangeCollection<Project> ProjectsList { get; set; }
        public ObservableRangeCollection<Project> ProfilProjectsList { get; set; }


        public ObservableRangeCollection<ProfilResponse> SquadList { get; set; }
        public ObservableRangeCollection<ProfilResponse> AddMembersList { get; set; }
        public ObservableRangeCollection<ProfilResponse> SearchMembersList { get; set; }


        public int NumberOfProjects { get; set; }

        public string  ProjectOwnerName { get; set; }

        public string SearchPartKeyword { get; set; }

        public string SearchWord { get; set; }

        public int SelectedProjetId { get; set; }

        public Project SelectedProjet { get; set; }

        public bool IsProjectOwner { get; set; }


        private Project AddProjectCell;

        public int MyNotes { get; set; } = 0;

        public ProjectViewModel()
        {

        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            GetAllProjects();
            GetProfilProjects();


            this.PropertyChanged += (s, e) =>
            {
                if (
                e.PropertyName == nameof(SearchPartKeyword)
                )
                {
                    //Search(SearchWord);
                    SearchCommand.Execute(null);
                }
            };

        }

        public async void GetAllProjects()
        {
            try
            {
                var result = await App.AppServices.GetProjects();
                ProjectsList = new ObservableRangeCollection<Project>(result.data.ToList());

                if(ProjectsList.Any())
                {
                    SelectedProjetId = (int)(ProjectsList?.FirstOrDefault()?.Id);
                    ProjectsList[0].IsSelected = true;
                    GetProjectSquad(SelectedProjetId);
                }
                NumberOfProjects = ProjectsList.Count;
                //SelectedProjetId = SelectedProjetId;


            }
            catch (Exception ex)
            {

            }
        }

        public async void GetProfilProjects()
        {
            try
            {
                var result = await App.AppServices.GetProfilProjects();
                ProfilProjectsList = new ObservableRangeCollection<Project>(result.data.ToList());

                if (ProfilProjectsList.Any())
                {
                    SelectedProjetId = (int)(ProfilProjectsList?.FirstOrDefault()?.Id);
                    ProfilProjectsList[0].IsSelected = true;
                    GetProjectSquad(SelectedProjetId);
                }
                NumberOfProjects = ProjectsList.Count;
                //SelectedProjetId = SelectedProjetId;

            }
            catch (Exception ex)
            {

            }
        }

        public async void GetProjectSquad(long ProjectId)
        {
            try
            {
                AppHelpers.LoadingShow();
                var result = await App.AppServices.GetProjectSquad(ProjectId);
                var result2 = await App.AppServices.GetProjectStaffMembersToAdd(ProjectId);
                AppHelpers.LoadingHide();
                SquadList = new ObservableRangeCollection<ProfilResponse>(result.data.ToList());
                AddMembersList = new ObservableRangeCollection<ProfilResponse>(result2.data.ToList());
                SearchMembersList = AddMembersList;

                IsProjectOwner = SelectedProjet.OwnerBy == AppPreferences.UserId;

                foreach (ProfilResponse profil in SquadList)
                {
                    if (profil.IsOwner)
                    {
                        ProjectOwnerName = profil.FirstName + " " + profil.LastName;
                        break;
                    }

                }
                    ProjectsList.FirstOrDefault(x => x.Id == ProjectId).OwnerName = ProjectOwnerName;
            }
            catch (Exception ex)
            {

            }
        }
        
        private bool canAddProjectCommand = true;
        public ICommand AddProjectCommand => new Command(async () =>
        {
            try
            {
                canAddProjectCommand = false;

                App.Current.MainPage.Navigation.PushAsync(new NewProjectPage());

                //var result = await App.AppServices.PostProject(projectRequest);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                canAddProjectCommand = true;
            }

        },
() => canAddProjectCommand);


        private bool canSelectProject = true;
        public ICommand SelectProjectCommand => new Command<Project>(async (model) =>
         {
             try
             {
                 canSelectProject = false;

                 SelectedProjetId = model.Id;

                 GetProjectSquad(model.Id);
                 foreach (Project project in ProjectsList)
                     project.IsSelected = false;
                 foreach (Project project in ProfilProjectsList)
                     project.IsSelected = false;

                 model.IsSelected = true;

             }
             catch (Exception ex)
             {

             }
             finally
             {
                 canSelectProject = true;
             }
         }
        ,
        (_) => canSelectProject);

        private bool canAddProject = true;
        public ICommand AddProjectCommmand => new Command(async () =>
        {
            try
            {
                canAddProject = false;
                App.Current.MainPage.Navigation.PushAsync(new NewProjectPage());

            }
            catch (Exception ex)
            {

            }
            finally
            {
                canAddProject = true;
            }
        },
          () => canAddProject);

        private bool canSelectProfil = true;
        public ICommand SelectProfilCommand => new Command<ProfilResponse>(async (model) =>
        {
            try
            {
                canSelectProfil = false;
                model.IsSelected = ! model.IsSelected;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                canSelectProfil = true;
            }
        }
       ,
       (_) => canSelectProfil);


        private AddMembersPopup addMembersPopup;
        private bool canOpenAddMembersPopup = true;

        public ICommand OpenAddMembersPopupCommand => new Command(async () =>
        {
            try
            {
                canOpenAddMembersPopup = false;

                if (addMembersPopup == null)
                    addMembersPopup = new AddMembersPopup() { BindingContext = this };

                await PopupNavigation.Instance.PushSingleAsync(addMembersPopup);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                canOpenAddMembersPopup = true;
            }


        }, () => canOpenAddMembersPopup);

        private ProfilDetailsPopup profilDetailsPopup;
        private bool canProfilDetailsPopup = true;

        public ICommand OpenProfilDetailsPopupCommand => new Command<ProfilResponse>(async (model) =>
        {
            try
            {
                canProfilDetailsPopup = false;

                if (profilDetailsPopup == null)
                    profilDetailsPopup = new ProfilDetailsPopup() { BindingContext = this };

                await PopupNavigation.Instance.PushSingleAsync(profilDetailsPopup);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                canProfilDetailsPopup = true;
            }


        }, (_) => canProfilDetailsPopup);

        public void Search(string word)
        {

            try
            {
                if (word.Length > 0)
                {
                    SearchMembersList = new ObservableRangeCollection<ProfilResponse>();
                    foreach (ProfilResponse profil in AddMembersList)
                    {
                        if (profil.FirstName.ToLower().Contains(word.ToLower()) || profil.LastName.ToLower().Contains(word.ToLower()))
                        {
                            SearchMembersList.Add(profil);
                        }
                    }
                }

                else if(word.Length == 0)
                    SearchMembersList = AddMembersList;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }

        }

        private bool CanSearch = true;
        public ICommand SearchCommand => new Command(() =>
        {
            try
            {
                CanSearch = false;

 
                SearchMembersList = new ObservableRangeCollection<ProfilResponse>();
                var key = (SearchPartKeyword ?? "").ToLower();
                if(AddMembersList?.Count > 0)
                {
                    var list = AddMembersList.Where(member => (member.LastName + member.FirstName ?? "").ToLower().Contains(key));
                    SearchMembersList.ReplaceRange(list);
                }
            }
            catch (Exception ex)
            {
                //Logger?.LogError(ex, showError: true);
            }
            finally
            {
                CanSearch = true;
            }
        }, () => CanSearch);

        private bool canAddMembers = true;
        public ICommand AddMembers => new Command(async() =>
        {
            try
            {
                canAddMembers = false;
                var addMemberslist = AddMembersList.Where(member => (member.IsSelected == true)).Select(x => x.RecId)?.ToList();
                AddMembersRequest addMemebersRequest = new AddMembersRequest()
                {
                    projectId = SelectedProjetId,
                    members = addMemberslist

                };
                AppHelpers.LoadingShow();
                var result =await App.AppServices.PostMembers(addMemebersRequest);
                AppHelpers.LoadingHide();
                GetProjectSquad(SelectedProjetId);
                PopupNavigation.Instance.PopAllAsync();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                canAddMembers = true;
            }
        }
        ,
        () => canAddMembers);

        private bool canRemoveMember = true;
        public ICommand RemoveMemberCommand => new Command<ProfilResponse>(async (model) =>
        {
            try
            {
                canRemoveMember = false;


            }
            catch (Exception ex)
            {

            }

        },
        (_)=> canRemoveMember);




        private bool canChangePercent = true;
        public ICommand ChangePercentCommand => new Command(async () =>
        {
            try
            {
                canChangePercent = true;

            }
            catch (Exception ex)
            {

            }
        }, () => canChangePercent);

        private ChangePercentPopup changePercentPopup;
        private bool canOpenChangePercentPopup = true;
        public ICommand OpenChangePercentPopup => new Command(async () =>
        {
            try
            {
                canOpenChangePercentPopup = false;
                if (changePercentPopup == null)
                    changePercentPopup = new ChangePercentPopup() { BindingContext = this };

                await PopupNavigation.Instance.PushSingleAsync(changePercentPopup);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                canOpenChangePercentPopup = true;
            }
        }
        ,() => canOpenChangePercentPopup);

        private AddPointsPopup addPointsPopup;
        private bool canAddPoints = true;
        public ICommand AddPointsCommand => new Command<ProfilResponse>(async (model) =>
        {
            try
            {
                canAddPoints = false;
                if(addPointsPopup == null)
                    addPointsPopup = new AddPointsPopup() { BindingContext = this };
                await PopupNavigation.Instance.PopAsync();
                await PopupNavigation.Instance.PushSingleAsync(addPointsPopup);



            }
            catch (Exception ex)
            {

            }
            finally
            {
                canAddPoints = true;
            }

        },
        (_) => canAddPoints); 

    }
}

