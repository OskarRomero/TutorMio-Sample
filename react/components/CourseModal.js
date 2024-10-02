import React from "react";
import Modal from "react-bootstrap/Modal";
import Col from "react-bootstrap/Col";
import CourseClasses from "../../components/course-classes/CourseClasses";

function CourseModal(props) {
  const isShow = props.toggleShow;
  const modalData = props.modalData;

  return (
    <>
      <Modal
        show={isShow}
        onHide={isShow}
        dialogClassName={null}
        size="lg"
        scrollable={true}
      >
        <Modal.Header closeButton style={{ backgroundColor: "light" }}>
          <Col className="col-5">
            {" "}
            <img
              src={modalData.imgUrl}
              style={{ height: "40px", width: "40px" }}
              alt=""
              className="rounded-circle "
            />
          </Col>
          <Col>
            <h5>{modalData.name}</h5>
          </Col>
        </Modal.Header>
        <Modal.Body>
          <div className="text-start mt-3">
            <CourseClasses
              modalData={modalData}
              notifyApp={props.notifyApp}
              targetClass={props.targetClass}
              wishlist={props.wishlist}
              notifyAppRemove={props.notifyAppRemove}
              addBtn={props.addBtn}
              removeBtn={props.removeBtn}
              sessionHrs={props.sessionHrs}
              language={props.language}
            />
          </div>
        </Modal.Body>
      </Modal>
    </>
  );
}

export default CourseModal;
