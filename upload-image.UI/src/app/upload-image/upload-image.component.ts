import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http'

@Component({
  selector: 'app-upload-image',
  templateUrl: './upload-image.component.html',
  styleUrls: ['./upload-image.component.css']
})
export class UploadImageComponent implements OnInit {
  // define as variáveis
  fileToUpload: File;
  form: FormGroup
  selectedImage: string;
  imageUrl: string;

  // define o construtor da classe
  constructor(
    private http: HttpClient,
    private formBuilder: FormBuilder
  ) { }

  // define o que a classe irá fazer assim que iniciar
  ngOnInit() {
    this.FormSetup();
  }

  // inicia a instância do meu formulário
  FormSetup() {
    this.form = this.formBuilder.group({
      name: ['', [Validators.required]],
      img: ['', [Validators.required]]
    });
  }

  // trata a imagem assim que ela é selecionada
  handleFileInput(file: FileList) {
    this.fileToUpload = file.item(0);

    var reader = new FileReader();

    reader.onload = (event: any) => {
      this.selectedImage = this.fileToUpload.name;
    }
    reader.readAsDataURL(this.fileToUpload);
  }

  // manda a imagem para a API através de uma requisição HTTP POST
  salvar() {
    const dataForm = this.form.getRawValue();

    const formData: FormData = new FormData();
    formData.append('Image', this.fileToUpload, this.fileToUpload.name);
    formData.append('ImageName', dataForm['name']);

    this.http.post('https://localhost:44381/UploadImage', formData).subscribe(
      success => {
        this.selectedImage = '';
        this.form.reset();
      }, error => {
        console.log(error);
      }
    );
  }

}
