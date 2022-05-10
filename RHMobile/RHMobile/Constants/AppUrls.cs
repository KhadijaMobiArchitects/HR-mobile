﻿using System;
namespace XForms.Constants
{
    public class AppUrls
    {
        //public const string BaseUrl = "https://rh-api-dev.azurewebsites.net/api/";
        public const string BaseUrl = "http://rh-api-dev-mobiarchitects.azurewebsites.net/api/";

        

        public const string Singin = BaseUrl + "Account/Authenticate";

        public const string GesRequestsListLeave = BaseUrl + "leaves/getleaves/";
        public const string GetRequestListTypeLeave = BaseUrl + "RefTypesLeave/";
        public const string GetRequestListProject = BaseUrl + "Projects/GetProjects";
        public const string GetRequestListProfilProject = BaseUrl + "Projects/GetProfilProjects";
        public const string GetRequestSituationProject = BaseUrl + "RefSituationsProject/";
        public const string GetRequestProjectSquad = BaseUrl + "projects/getSquad/";
        public const string GetRequestProfils = BaseUrl + "Profils/GetProfils";




        public const string PostLeaveRequest = BaseUrl + "leaves/postLeave/";
        public const string DeleteLeaveRequest = BaseUrl + "leaves/deleteLeave";
        public const string PostProjectRequest = BaseUrl + "projects/CreateProject";
        public const string GetStaffMembersToAddRequest = BaseUrl + "Projects/GetStaffMembersToAdd";
        public const string PostMembersRequest = BaseUrl + "Projects/AddStaffMembers";




        //public const string PostRequestLeave = "http://localhost:3000/Leaves";



    }
}
