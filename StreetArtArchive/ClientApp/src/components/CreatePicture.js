import React, {Component} from 'react';
import {Button, Col, Form, FormGroup, Input, InputGroup, Label, Row} from "reactstrap";

export class CreatePicture extends Component {
  static displayName = CreatePicture.name;

  constructor(props) {
    super(props);
    this.state = {categories : [], imageUrl:"", image: null}
    this.addCategory = this.addCategory.bind(this);
    this.submit = this.submit.bind(this);
    this.onChangeName = this.onChangeName.bind(this);
    this.onChangeValue = this.onChangeValue.bind(this);
    this.onChangeImage = this.onChangeImage.bind(this);
    this.removeCategory = this.removeCategory.bind(this);
  }
  
  addCategory(){
    this.setState({categories: [...this.state.categories, { name: "", value: ""}], });
  }

  removeCategory(e, index){
    let values = [...this.state.categories];
    values.splice(index,1);
    this.setState({categories: values, });
  }
  
  submit(e){
    e.preventDefault();
    const data = {
      image: this.state.image,
      categories: this.state.categories
    }
    console.log(data);
  }

  onChangeName(e, index){
    const values = [...this.state.categories];
    values[index].name = e.target.value;
    this.setState({categories: values });
  }

  onChangeValue(e, index){
    const values = [...this.state.categories];
    values[index].value = e.target.value;
    this.setState({categories: values });
  }

  onChangeImage(e){
    if(e.target.files.length > 0) {
      const imageUrl = URL.createObjectURL(e.target.files[0]);
      this.setState({imageUrl: imageUrl, image: e.target.files[0]});
    }
    else{
      this.setState({imageUrl: "", image: null});
    }
  }

  render() {
    return (
      <div>
        <h1 id="tableLabel">Create Picture</h1>
        <div>
          <img alt="" src={this.state.imageUrl}/>
        </div>
        <Form onSubmit={this.submit}>
          <Input type="file" accept="image/*" onChange={this.onChangeImage}/>
          {this.state.categories.map((category, index) =>
            <Row className="row-cols-lg-auto g-3 align-items-center">
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
                    <Input value={category.value} onChange={(e => this.onChangeValue(e, index))}/>
                    <Button color="danger" type="button" onClick={e => this.removeCategory(e,index)}>Remove</Button>
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