import React, { useState } from "react";
import { useLocation } from "react-router-dom";

function New() {
  const location = useLocation();
  const [activeId] = useState(location?.state?.payload?.id || null);
  const [course] = useState({
    id: "",
    name: "",
    imgUrl: "",
    description: "",
    language: "",
  });
  // console.log("activeId", activeId);
  console.log("location", location);
  false && console.log(course);
  console.log(activeId);

  return (
    <div className="container my-5 fs-2">
      <div className="row">
        {/* Page Title */}
        <div className="col-sm">
          {activeId ? (
            <h1>Edit {location.state.payload.name}</h1>
          ) : (
            <h1>Add new course</h1>
          )}
        </div>
        {/* form section */}
        <div className="col-md"></div>
      </div>
    </div>
  );
}
export default New;
