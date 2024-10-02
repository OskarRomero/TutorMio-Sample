import React, { useEffect, useState, lazy, Suspense } from "react";
import Container from "react-bootstrap/Container";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";
import "./courses.css";
import Course from "./Course";
import { useLocation, useRouteLoaderData, useNavigate } from "react-router-dom";
import coursesService from "../../services/coursesService";
import { GrNext } from "react-icons/gr";
import { Link } from "react-router-dom";

const CourseModal = lazy(() => import("./CourseModal"));

function Courses(props) {
  const navigate = useNavigate();
  const token = useRouteLoaderData("root");
  const location = useLocation();
  const [cards, setCards] = useState({
    spanish: [],
    english: [],
  });
  const [isShow, setIsShow] = useState(false);
  const [activeCourse, setActiveCourse] = useState({
    course: {},
    classToScrollTo: {},
  });
  const [targetClass] = useState(location?.state?.courseClass || null);
  useEffect(() => {
    if (
      Object.keys(props.tunnelingCourseTranslation[props.language]).length === 0
    ) {
      import("./translation").then((module) => {
        props.bubblingCourseTranslation(module[props.language], props.language);
      });
    }
    // eslint-disable-next-line
  }, [props.language]);

  useEffect(() => {
    if (props.tunnelingCourses[props.language].length === 0) {
      coursesService
        .getCoursesByLangv2(props.language)
        .then(onGetCoursesSuccess)
        .catch(onGetCoursesErr);
    }
    // eslint-disable-next-line
  }, [props.language]);

  useEffect(() => {
    let lang = props.language;
    if (props.tunnelingCourses[lang].length > 0) {
      setCards((prevState) => {
        let newState = { ...prevState };
        newState[lang] = [...newState[lang]];
        newState[lang] = props.tunnelingCourses[lang].map(cardMapper);
        return newState;
      });
    }
    // eslint-disable-next-line
  }, [props.tunnelingCourses[props.language]]);

  useEffect(() => {
    if (targetClass !== null) {
      targetCourseSetter(targetClass.courseId, targetClass);
    } else return;
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [targetClass]);

  useEffect(() => {
    if (props.activeClass && Object.keys(props.activeClass).length !== 0) {
      const classToScrollTo = props.activeClass;
      const courseId = props.activeClass.courseId;
      targetCourseSetter(courseId, classToScrollTo);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.activeClass]);

  function onGetCoursesSuccess(response) {
    props.courses(response.data.items);
  }

  function onGetCoursesErr(err) {
    console.error(err);
  }

  function targetCourseSetter(targetCourseId, classToScrollTo) {
    const targetCourse = props.tunnelingCourses.find(courseFilter);
    function courseFilter(course) {
      return course.id === targetCourseId;
    }
    toggleModalHandler(targetCourse, classToScrollTo);
  }

  function toggleModalHandler(cardInfo, classToScrollTo) {
    if (cardInfo) {
      setActiveCourse((prevState) => {
        let newState = { ...prevState };
        newState.course = cardInfo;
        if (cardInfo && classToScrollTo) {
          newState.classToScrollTo = classToScrollTo;
        }
        return newState;
      });
    } else {
      props.onDisactivateCurrentClass({});
      setActiveCourse({});
    }
    setIsShow(!isShow);
  }
  function cardMapper(data) {
    return (
      <Course
        key={`courseKey:${data.id}`}
        cardInfo={data}
        toggleModal={toggleModalHandler}
        viewBtn={props.tunnelingCourseTranslation[props.language].viewBtn}
        isAuthorized={token ? true : false}
      />
    );
  }

  function onAddNewCourse() {
    navigate("new");
  }

  return (
    <>
      {isShow && (
        <Suspense fallback={<p style={{ position: "absolute" }}>loading...</p>}>
          <CourseModal
            toggleShow={toggleModalHandler}
            modalData={activeCourse.course}
            notifyApp={props.notifyApp}
            targetClass={activeCourse.classToScrollTo}
            wishlist={props.wishlist}
            notifyAppRemove={props.notifyAppRemove}
            addBtn={props.tunnelingCourseTranslation[props.language].addBtn}
            removeBtn={
              props.tunnelingCourseTranslation[props.language].removeBtn
            }
            sessionHrs={
              props.tunnelingCourseTranslation[props.language].sessionHrs
            }
            language={props.language}
          />
        </Suspense>
      )}
      <section
        id="courses-cls-size"
        className="py-3 pricing-bg-color border-top border-bottom border-light "
      >
        {/*               className={token ? "col d-flex justify-content-end" : ""}
         */}
        <Container>
          <Row>
            <Col></Col>
            <Col lg={8}>
              <div className="text-center">
                <h1 className="mt-0">
                  <i className="mdi mdi-tag-multiple"></i>
                </h1>
                <h3 className="course-font header-wt">
                  {props.tunnelingCourseTranslation[props.language].title}
                </h3>
                <p className=" mt-2 course-font">
                  {props.tunnelingCourseTranslation[props.language].subtitle}
                </p>
              </div>
            </Col>
            {token && (
              <Col>
                <div className="col d-flex justify-content-center pb-3">
                  <button className="btn btn-primary" onClick={onAddNewCourse}>
                    Add New Course
                  </button>
                </div>
              </Col>
            )}
          </Row>

          <Row>
            {(props.language === "spanish" && cards.spanish) ||
              (props.language === "english" && cards.english)}
          </Row>
          <Row>
            <Col className="d-flex justify-content-center">
              {props.wishlist.length > 0 && (
                <Link
                  to={"/plans"}
                  className="btn btn-outline-primary btn-md  btn-rounded"
                >
                  <span className="mx-0 px-0">
                    {(props.language === "english" && "Continue To Plans") ||
                      "Continuar a Planes"}
                  </span>{" "}
                  <GrNext className="mx-0 px-0" />
                </Link>
              )}
            </Col>
          </Row>
        </Container>
      </section>
    </>
  );
}

export default Courses;
