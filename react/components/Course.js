import React from "react";
import Col from "react-bootstrap/Col";
import Card from "react-bootstrap/Card";
import Row from "react-bootstrap/Row";
import "./course-card.css";
import { useNavigate } from "react-router-dom";

function Course(props) {
  const navigate = useNavigate();
  const isAuthorized = props.isAuthorized;
  const cardInfo = props.cardInfo;
  function viewMoreHandler() {
    props.toggleModal(cardInfo);
  }
  function onEditHandler() {
    const stateForTransport = { type: "COURSE_OBJ", payload: cardInfo };
    navigate(`new/${cardInfo.id}`, {
      type: "COURSE_OBJ",
      state: stateForTransport,
    });
  }
  return (
    <Col sm={6} lg={3} className="pb-4">
      <Card className="courses-card-cls-size">
        <Card.Img
          src={cardInfo.imgUrl}
          id="course-thumb"
          className="cls-course-img"
        />
        <Card.Body className="text-center">
          <Card.Title as="h5">{cardInfo.name}</Card.Title>
          <div className="pb-2">
            <button
              className={`btn mt-2 btn-outline-secondary ${
                isAuthorized ? "" : "stretched-link"
              }`}
              onClick={viewMoreHandler}
            >
              {props.viewBtn}
            </button>
          </div>
          {isAuthorized && (
            <Row>
              <div className="col">
                <button className="btn btn-danger">Delete</button>
              </div>
              <div className="col">
                <button className="btn btn-warning" onClick={onEditHandler}>
                  Edit
                </button>
              </div>
            </Row>
          )}

          <div className="row"></div>
        </Card.Body>
      </Card>
    </Col>
  );
}
export default Course;
