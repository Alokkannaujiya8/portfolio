import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';

import { DashboardComponent } from './dashboard/dashboard.component';
import { ProfileAdminComponent } from './profile/profile-admin.component';
import { ProjectsAdminComponent } from './projects/projects-admin.component';
import { SkillsAdminComponent } from './skills/skills-admin.component';
import { ExperienceAdminComponent } from './experience/experience-admin.component';
import { EducationAdminComponent } from './education/education-admin.component';
import { ResumeAdminComponent } from './resume/resume-admin.component';
import { CertificatesAdminComponent } from './certificates/certificates-admin.component';
import { ServicesAdminComponent } from './services/services-admin.component';
import { BlogsAdminComponent } from './blogs/blogs-admin.component';
import { GalleryAdminComponent } from './gallery/gallery-admin.component';
import { MessagesAdminComponent } from './messages/messages-admin.component';
import { SettingsAdminComponent } from './settings/settings-admin.component';
import { CommentsAdminComponent } from './comments/comments-admin.component';
import { ResumeViewsComponent } from './resume-views/resume-views.component';
import { NotificationsListComponent } from './notifications/notifications-list.component';

const routes: Routes = [
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
];

const ADMIN_COMPONENTS = [
  DashboardComponent,
  ProfileAdminComponent,
  CommentsAdminComponent,
  ResumeViewsComponent,
  NotificationsListComponent,
  ProjectsAdminComponent,
  SkillsAdminComponent,
  ExperienceAdminComponent,
  EducationAdminComponent,
  ResumeAdminComponent,
  CertificatesAdminComponent,
  ServicesAdminComponent,
  BlogsAdminComponent,
  GalleryAdminComponent,
  MessagesAdminComponent,
  SettingsAdminComponent
];

@NgModule({
  declarations: [
    ...ADMIN_COMPONENTS
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    ...ADMIN_COMPONENTS
  ]
})
export class DashboardModule { }
