import React, {Component} from 'react';
import {
  Button,
  Card,
  CardBody,
  CardText,
  CardTitle,
  Col,
  Modal,
  ModalBody,
  ModalFooter, ModalHeader,
  Row,
  Spinner
} from 'reactstrap';
import InfiniteScroll from 'react-infinite-scroller';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = {pictures: [], page: 0, hasMore: true, loading: false, modal: false, selectedPictureId: ''};
    this.populatePictures = this.populatePictures.bind(this);
  }

  selectPictureForRemoval = (id) => {
    this.setState({selectedPictureId: id});
    this.toggleDelete();
  }
  
  deletePicture = async () => {
    try {
      const response = await fetch('pictures/DeleteById?id=' + this.state.selectedPictureId, {method: 'DELETE'});
      if (response.ok) {
        const index = this.state.pictures.findIndex(el => el.id === this.state.selectedPictureId)
        this.state.pictures.splice(index, index >= 0 ? 1 : 0)
      } else {
        console.log(response);
      }
    }
    finally {
      this.toggleDelete();      
    }
  }
  
  toggleDelete = () => {
    this.setState({modal: !this.state.modal});
  }

  async populatePictures() {
    if (this.state.loading) {
      return;
    }

    this.setState({loading: true});

    try {
      const response = await fetch('pictures/GetList?page=' + this.state.page);
      const data = await response.json();
      this.setState({
        pictures: [...this.state.pictures, ...data.pictures],
        page: this.state.page + 1,
        hasMore: data.hasMore
      })
    } catch (e) {
      this.setState({hasMore: false});
    } finally {
      this.setState({loading: false});
    }
  }

  render() {
    return (
      <div>
        <h1 id="tableLabel">Pictures</h1>

        <InfiniteScroll
          loadMore={this.populatePictures}
          hasMore={this.state.hasMore}
          loader={<Spinner color="secondary" key={0}>Loading...</Spinner>}
        >
          <Row xs={3}>
            {this.state.pictures.map(picture =>
              <Col key={picture.id}>
                <Card body>
                  <img
                    alt="No Thumbnail"
                    src={"data:image/png;base64," + picture.thumbnail.data}
                    width="100%"
                  />
                  <CardBody>
                    <CardTitle tag="h5">
                      {picture.categories.find(c => c.name === "Name")?.values[0]}
                    </CardTitle>
                    <CardText>
                      {picture.categories.find(c => c.name === "Year")?.values[0]} {picture.categories.find(c => c.name === "Author")?.values}
                    </CardText>
                  </CardBody>
                  <CardBody style={{display: 'flex'}}>
                    <Button color="primary" outline href={"/create?id=" + picture.id}>
                      Edit
                    </Button>
                    <Button style={{marginLeft: 'auto'}} color="danger" outline onClick={_ => this.selectPictureForRemoval(picture.id)}>
                      Delete
                    </Button>
                  </CardBody>
                </Card>
              </Col>
            )}
          </Row>
        </InfiniteScroll>
        <Modal centered isOpen={this.state.modal} toggle={this.toggleDelete}>
          <ModalHeader toggle={this.toggleDelete}>Delete Picture</ModalHeader>
          <ModalBody>
            Last chance to change your mind!
          </ModalBody>
          <ModalFooter>
            <Button color="danger" onClick={this.deletePicture}>
              Delete
            </Button>{' '}
            <Button color="secondary" onClick={this.toggleDelete}>
              Cancel
            </Button>
          </ModalFooter>
        </Modal>
      </div>
    );
  }
}
