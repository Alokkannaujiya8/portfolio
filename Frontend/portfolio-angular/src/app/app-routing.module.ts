import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PublicLayoutComponent } from './shared/layouts/public-layout/public-layout.component';
import { HomeComponent } from './features/home/home.component';
import { LoginComponent } from './features/login/login.component';
import { AdminLayoutComponent } from './shared/layouts/admin-layout/admin-layout.component';
import { DashboardComponent } from './features/admin/dashboard/dashboard.component';
import { ProfileAdminComponent } from './features/admin/profile/profile-admin.component';
import { CommentsAdminComponent } from './features/admin/comments/comments-admin.component';
import { ResumeViewsComponent } from './features/admin/resume-views/resume-views.component';
import { ProjectsAdminComponent } from './features/admin/projects/projects-admin.component';
import { SkillsAdminComponent } from './features/admin/skills/skills-admin.component';
import { ExperienceAdminComponent } from './features/admin/experience/experience-admin.component';
import { EducationAdminComponent } from './features/admin/education/education-admin.component';
import { ResumeAdminComponent } from './features/admin/resume/resume-admin.component';
import { CertificatesAdminComponent } from './features/admin/certificates/certificates-admin.component';
import { ServicesAdminComponent } from './features/admin/services/services-admin.component';
import { BlogsAdminComponent } from './features/admin/blogs/blogs-admin.component';
import { GalleryAdminComponent } from './features/admin/gallery/gallery-admin.component';
import { MessagesAdminComponent } from './features/admin/messages/messages-admin.component';
import { SettingsAdminComponent } from './features/admin/settings/settings-admin.component';
import { NotificationsListComponent } from './features/admin/notifications/notifications-list.component';
import { authGuard } from './core/guards/auth.guard';


const routes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { path: '', component: HomeComponent }
    ]
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'profile', component: ProfileAdminComponent },
      { path: 'projects', component: ProjectsAdminComponent },
      { path: 'skills', component: SkillsAdminComponent },
      { path: 'experience', component: ExperienceAdminComponent },
      { path: 'education', component: EducationAdminComponent },
      { path: 'certificates', component: CertificatesAdminComponent },
      { path: 'services', component: ServicesAdminComponent },
      { path: 'blogs', component: BlogsAdminComponent },
      { path: 'categories', component: SettingsAdminComponent },
      { path: 'comments', component: CommentsAdminComponent },
      { path: 'gallery', component: GalleryAdminComponent },
      { path: 'resume', component: ResumeAdminComponent },
      { path: 'messages', component: MessagesAdminComponent },
      { path: 'resume-views', component: ResumeViewsComponent },
      { path: 'consumer-views', component: DashboardComponent },
      { path: 'creator-views', component: DashboardComponent },
      { path: 'ai-video', component: DashboardComponent },
      { path: 'notifications', component: NotificationsListComponent },
      { path: 'settings', component: SettingsAdminComponent }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
