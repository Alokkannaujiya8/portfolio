import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // --- Public Endpoints ---
  getHero(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/home/hero`);
  }

  getAbout(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/home/about`);
  }

  getProjects(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/projects`);
  }

  getSkills(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/skills`);
  }

  getExperiences(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/experiences`);
  }

  getEducations(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/educations`);
  }

  getCertificates(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/certificates`);
  }

  getServices(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/services`);
  }

  getBlogs(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/blogs?onlyPublished=true`);
  }

  getBlogBySlug(slug: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/blogs/${slug}`);
  }

  getGallery(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/gallery`);
  }

  getSEO(pageName: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/seo/${pageName}`);
  }

  getGithubRepos(username: string): Observable<any[]> {
    return this.http.get<any[]>(`https://api.github.com/users/${username}/repos`);
  }

  submitContact(message: { name: string; email: string; subject: string; message: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/contact`, message);
  }

  downloadResumeUrl(): string {
    return `${this.apiUrl}/public/resume/download`;
  }

  // --- Administrative Endpoints ---
  updateHero(hero: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/admin/home/hero`, hero);
  }

  updateAbout(about: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/admin/home/about`, about);
  }

  createProject(project: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/projects`, project);
  }

  updateProject(id: string, project: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/projects/${id}`, project);
  }

  deleteProject(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/projects/${id}`);
  }

  createSkill(skill: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/skills`, skill);
  }

  updateSkill(id: string, skill: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/skills/${id}`, skill);
  }

  deleteSkill(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/skills/${id}`);
  }

  createExperience(exp: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/experiences`, exp);
  }

  updateExperience(id: string, exp: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/experiences/${id}`, exp);
  }

  deleteExperience(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/experiences/${id}`);
  }

  createEducation(edu: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/educations`, edu);
  }

  updateEducation(id: string, edu: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/educations/${id}`, edu);
  }

  deleteEducation(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/educations/${id}`);
  }

  createCertificate(cert: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/certificates`, cert);
  }

  updateCertificate(id: string, cert: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/certificates/${id}`, cert);
  }

  deleteCertificate(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/certificates/${id}`);
  }

  createService(service: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/services`, service);
  }

  updateService(id: string, service: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/services/${id}`, service);
  }

  deleteService(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/services/${id}`);
  }

  getAllBlogsAdmin(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/blogs?onlyPublished=false`);
  }

  createBlog(blog: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/blogs`, blog);
  }

  updateBlog(id: string, blog: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/blogs/${id}`, blog);
  }

  deleteBlog(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/blogs/${id}`);
  }

  createGalleryItem(item: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/gallery`, item);
  }

  deleteGalleryItem(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/gallery/${id}`);
  }

  getMessages(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/contact/messages`);
  }

  markMessageRead(id: string): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/admin/contact/messages/${id}/read`, {});
  }

  getSettings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/settings`);
  }

  updateSettings(settings: { [key: string]: string }): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/admin/settings`, settings);
  }

  getSEOs(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/seo`);
  }

  updateSEO(seo: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/admin/seo`, seo);
  }

  getResumes(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/resume`);
  }

  uploadResume(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post<any>(`${this.apiUrl}/admin/resume/upload`, formData);
  }

  activateResume(id: string): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/admin/resume`, { id });
  }

  deleteResume(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/admin/resume/${id}`);
  }

  uploadImage(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post<any>(`${this.apiUrl}/admin/upload/image`, formData);
  }
}
