import React, {Component} from 'react';
import { useNavigate, useParams } from "react-router-dom";
import {Button, Col, Form, FormGroup, Input, InputGroup, Label, Row} from "reactstrap";

class CreatePictureComponent extends Component {
  static displayName = CreatePicture.name;

  constructor(props) {
    super(props);
    this.id = props.id;
    this.state = {categories: [], imageUrl: "", image: null}
    this.addCategory = this.addCategory.bind(this);
    this.submit = this.submit.bind(this);
    this.onChangeName = this.onChangeName.bind(this);
    this.onChangeValue = this.onChangeValue.bind(this);
    this.onChangeImage = this.onChangeImage.bind(this);
    this.removeCategory = this.removeCategory.bind(this);
  }
  
  componentDidMount() {
    let load = async () => {
      if(this.id !== '' && this.id != null){
        const response = await fetch('pictures/GetById?id=' + this.id);        
        let data = await response.json();

        const picture = await fetch('pictures/GetFullPicture?path=' + data.imagePath);        
        const pictureBlob = await picture.blob();        
        const pictureUrl = URL.createObjectURL(pictureBlob);
        
        data.image = pictureBlob;
        data.image.name = data.imagePath;
        data.imageUrl = pictureUrl;

        return data;
      }
      return {categories: [], imageUrl: "", image: null};
    }
    load().then(data => {
      this.setState({categories: data.categories, imageUrl: data.imageUrl, image: data.image});
    });
  }

  addCategory() {
    this.setState({categories: [...this.state.categories, {name: "", values: ""}],});
  }

  removeCategory(e, index) {
    let values = [...this.state.categories];
    values.splice(index, 1);
    this.setState({categories: values,});
  }

  async submit(e) {
    e.preventDefault();
    const { navigate } = this.props;
    const formData = new FormData();
    formData.append('image',this.state.image, this.state.image.name);
    this.state.categories.forEach((category, index) => {
      formData.append('categories['+index+'].name', category.name);
      formData.append('categories['+index+'].values', category.values);
    });
    let method = 'pictures/SavePicture';
    if(this.id){
      formData.append('id',this.id);
      method = 'pictures/UpdatePicture';
    }
    const config = {
      method: 'POST',
      body: formData
    }
    const response = await fetch(method, config);

    if (response.ok) {
      navigate('/fetch-data');
    } else {
      console.log(response);
    }
  }

  onChangeName(e, index) {
    const values = [...this.state.categories];
    values[index].name = e.target.value;
    this.setState({categories: values});
  }

  onChangeValue(e, index) {
    const values = [...this.state.categories];
    values[index].values = e.target.value;
    this.setState({categories: values});
  }

  onChangeImage(e) {
    if (e.target.files.length > 0) {
      const imageUrl = URL.createObjectURL(e.target.files[0]);
      this.setState({imageUrl: imageUrl, image: e.target.files[0]});
    } else {
      this.setState({imageUrl: "", image: null});
    }
  }

  render() {
    return (
      <div>
        <h1 id="tableLabel">{this.id ? "Edit Picture" : "Create Picture"}</h1>
        <div>
          <img style={{maxWidth : "100%" }} alt="" src={this.state.imageUrl}/>
        </div>
        <Form onSubmit={this.submit}>
          <Row className="row-cols-lg-auto g-3 align-items-center">
            <Col>
              <Input type="file" accept="image/*" onChange={this.onChangeImage}/>
            </Col>
          </Row>
          {this.state.categories.map((category, index) =>
            <Row key={index} className="row-cols-lg-auto g-3 align-items-center">
              <Col>
                <FormGroup>
                  <Label>Category Name</Label>
                  <Input value={category.name} onChange={(e => this.onChangeName(e, index))}/>
                </FormGroup>
              </Col>
              <Col>
                <FormGroup>
                  <Label>Category Value</Label>
                  <InputGroup>
                    <Input value={category.values} onChange={(e => this.onChangeValue(e, index))}/>
                    <Button color="danger" type="button" onClick={e => this.removeCategory(e, index)}>Remove</Button>
                  </InputGroup>
                </FormGroup>
              </Col>
            </Row>
          )}
          <Button type="button" color="success" onClick={this.addCategory}>Add Category</Button>
          <br/>
          <Button type="submit">Save</Button>
        </Form>
      </div>
    );
  }
}

export function CreatePicture(props) {
  const navigate = useNavigate();

  const { id } = useParams();

  return <CreatePictureComponent {...props} id={id} navigate={navigate} />;
}