import { AfterViewInit, Component, Inject, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatPaginator, MatTableDataSource, ShowOnDirtyErrorStateMatcher } from '@angular/material';
@Component({
  selector: 'app-landfill',
  templateUrl: './landfill.component.html'
})
export class LandfillComponent implements AfterViewInit {
  public users: Object[] = [];
  dataSource = new MatTableDataSource<Object>(this.users);
  displayedColumns:  string[] = [];
  httpClient: HttpClient;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;


  constructor(http: HttpClient) {
    http.get<Object>('https://192.168.2.148:45455/' + 'Package/GetAllPackages').subscribe(result => {
      console.log(result);  
    this.dataSource = new MatTableDataSource<Object>();
      this.ngAfterViewInit();
      
    }, error => console.error(error));
    this.httpClient = http;
    
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }
  
}
